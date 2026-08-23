#!/usr/bin/env bash
set -euo pipefail

sha="${1:?commit SHA required}"
api_root="/var/www/apis/rutacero"
web_root="/var/www/sitios/rutacero"

[[ "$sha" =~ ^[a-f0-9]{40}$ ]]

api_release="$api_root/releases/$sha"
web_release="$web_root/releases/$sha"

sudo install -d -o jesdav -g jesdav "$api_release" "$web_release" "$api_root/shared/storage"

sudo -u jesdav tar -xzf /tmp/api.tgz -C "$api_release"
sudo -u jesdav tar -xzf /tmp/web.tgz -C "$web_release"

previous_api=""
previous_web=""

if [[ -L "$api_root/current" ]]; then
    previous_api="$(readlink -f "$api_root/current" || true)"
fi

if [[ -L "$web_root/current" ]]; then
    previous_web="$(readlink -f "$web_root/current" || true)"
fi

sudo ln -sfn "$api_release" "$api_root/current"
sudo ln -sfn "$web_release" "$web_root/current"

sudo systemctl restart rutacero-api

healthy=false

for attempt in {1..15}; do
    if curl --fail --silent --show-error --max-time 5 http://127.0.0.1:5080/health >/dev/null 2>&1; then
        healthy=true
        break
    fi

    sleep 2
done

if [[ "$healthy" != true ]]; then
    if [[ -n "$previous_api" && -d "$previous_api" ]]; then
        sudo ln -sfn "$previous_api" "$api_root/current"
    fi

    if [[ -n "$previous_web" && -d "$previous_web" ]]; then
        sudo ln -sfn "$previous_web" "$web_root/current"
    fi

    sudo systemctl restart rutacero-api
    exit 1
fi

find "$api_root/releases" -mindepth 1 -maxdepth 1 -type d -printf '%T@ %p\n' |
    sort -nr |
    tail -n +6 |
    cut -d' ' -f2- |
    xargs -r sudo rm -rf --

find "$web_root/releases" -mindepth 1 -maxdepth 1 -type d -printf '%T@ %p\n' |
    sort -nr |
    tail -n +6 |
    cut -d' ' -f2- |
    xargs -r sudo rm -rf --