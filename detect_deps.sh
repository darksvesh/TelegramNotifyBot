#!/bin/sh
mkdir -p /deps
for bin in /app/*; do
    ldd $bin | tr -s '[:space:]' '\n' | grep -E '^/' | xargs -I{} cp -v --parents '{}' /deps
done