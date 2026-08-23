#!/usr/bin/env bash
set -euo pipefail
sha="${1:?commit SHA required}"
root="${VPS_APP_DIR:-/var/www/rutacero}"
[[ "$sha" =~ ^[a-f0-9]{40}$ ]]
[[ "$root" == /var/www/rutacero || "$root" == /srv/rutacero ]]
api_release="$root/api/releases/$sha"
web_release="$root/web/releases/$sha"
sudo install -d -o rutacero -g rutacero "$api_release" "$web_release" "$root/shared/storage"
sudo -u rutacero tar -xzf /tmp/api.tgz -C "$api_release"
sudo -u rutacero tar -xzf /tmp/web.tgz -C "$web_release"
previous_api="$(readlink -f "$root/api/current" || true)"
previous_web="$(readlink -f "$root/web/current" || true)"
sudo ln -sfn "$api_release" "$root/api/current"
sudo ln -sfn "$web_release" "$root/web/current"
sudo systemctl restart rutacero-api
if ! curl --fail --silent --show-error --max-time 15 http://127.0.0.1:5080/health >/dev/null; then
  [[ -n "$previous_api" && -n "$previous_web" ]]
  sudo ln -sfn "$previous_api" "$root/api/current"
  sudo ln -sfn "$previous_web" "$root/web/current"
  sudo systemctl restart rutacero-api
  exit 1
fi
find "$root/api/releases" -mindepth 1 -maxdepth 1 -type d -printf '%T@ %p\n' | sort -nr | tail -n +6 | cut -d' ' -f2- | xargs -r sudo rm -rf --
find "$root/web/releases" -mindepth 1 -maxdepth 1 -type d -printf '%T@ %p\n' | sort -nr | tail -n +6 | cut -d' ' -f2- | xargs -r sudo rm -rf --
