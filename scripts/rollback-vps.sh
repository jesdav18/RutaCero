#!/usr/bin/env bash
set -euo pipefail

api_root="/var/www/apis/rutacero"
web_root="/var/www/sitios/rutacero"

current_api="$(readlink -f "$api_root/current")"
current_web="$(readlink -f "$web_root/current")"

previous_api="$(find "$api_root/releases" -mindepth 1 -maxdepth 1 -type d ! -path "$current_api" -printf '%T@ %p\n' | sort -nr | head -1 | cut -d' ' -f2-)"
previous_web="$(find "$web_root/releases" -mindepth 1 -maxdepth 1 -type d ! -path "$current_web" -printf '%T@ %p\n' | sort -nr | head -1 | cut -d' ' -f2-)"

[[ -d "$previous_api" ]]
[[ -d "$previous_web" ]]

sudo ln -sfn "$previous_api" "$api_root/current"
sudo ln -sfn "$previous_web" "$web_root/current"

sudo systemctl restart rutacero-api

curl --fail --silent --show-error --max-time 15 http://127.0.0.1:5080/health >/dev/null
