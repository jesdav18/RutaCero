#!/usr/bin/env bash
set -euo pipefail
root="${VPS_APP_DIR:-/var/www/rutacero}"
[[ "$root" == /var/www/rutacero || "$root" == /srv/rutacero ]]
current_api="$(readlink -f "$root/api/current")"
current_web="$(readlink -f "$root/web/current")"
previous_api="$(find "$root/api/releases" -mindepth 1 -maxdepth 1 -type d ! -path "$current_api" -printf '%T@ %p\n' | sort -nr | head -1 | cut -d' ' -f2-)"
previous_web="$(find "$root/web/releases" -mindepth 1 -maxdepth 1 -type d ! -path "$current_web" -printf '%T@ %p\n' | sort -nr | head -1 | cut -d' ' -f2-)"
[[ -d "$previous_api" && -d "$previous_web" ]]
sudo ln -sfn "$previous_api" "$root/api/current"
sudo ln -sfn "$previous_web" "$root/web/current"
sudo systemctl restart rutacero-api
curl --fail --silent --show-error --max-time 15 http://127.0.0.1:5080/health >/dev/null
