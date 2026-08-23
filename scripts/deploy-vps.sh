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

previous_api="$(readlink -f "$api_root/current" || true)"
previous_web="$(readlink -f "$web_root/current" || true)"

sudo ln -sfn "$api_release" "$api_root/current"
sudo ln -sfn "$web_release" "$web_root/current"

sudo systemctl restart rutacero-api

if ! curl --fail --silent --show-error --max-time 15 http://127.0.0.1:5080/health >/dev/null; then
  [[ -n "$previous_api" && -n "$previous_web" ]]
  sudo ln -sfn "$previous_api" "$api_root/current"
  sudo ln -sfn "$previous_web" "$web_root/current"
  sudo systemctl restart rutacero-api
  exit 1
fi

find "$api_root/releases" -mindepth 1 -maxdepth 1 -type d -printf '%T@ %p\n' | sort -nr | tail -n +6 | cut -d' ' -f2- | xargs -r sudo rm -rf --
find "$web_root/releases" -mindepth 1 -maxdepth 1 -type d -printf '%T@ %p\n' | sort -nr | tail -n +6 | cut -d' ' -f2- | xargs -r sudo rm -rf --
