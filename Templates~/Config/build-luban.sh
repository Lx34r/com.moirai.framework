#!/bin/bash
set -e

LUBAN_REPO="https://github.com/focus-creative-games/luban.git"
LUBAN_DIR="../../luban"

# Step 1: Check if luban source directory exists
if [ ! -d "$LUBAN_DIR/.git" ]; then
    echo "[INFO] Luban source not found, cloning from $LUBAN_REPO ..."
    git clone "$LUBAN_REPO" "$LUBAN_DIR"
else
    # Step 2: Check for remote updates
    echo "[INFO] Checking for updates ..."
    cd "$LUBAN_DIR"
    if git fetch origin 2>/dev/null; then
        LOCAL_REV=$(git rev-parse HEAD)
        REMOTE_REV=$(git rev-parse origin/HEAD 2>/dev/null || echo "")
        if [ "$LOCAL_REV" = "$REMOTE_REV" ]; then
            echo "[INFO] Already up-to-date, skipping pull."
        else
            echo "[INFO] Updates found, pulling ..."
            git pull origin || echo "[WARN] Git pull failed, proceeding with local source."
        fi
    else
        echo "[WARN] Git fetch failed, proceeding with local source."
    fi
    cd - > /dev/null
fi

# Step 3: Clean previous build output
[ -d Luban ] && rm -rf Luban

# Step 4: Build
echo "[INFO] Building Luban ..."
dotnet build "$LUBAN_DIR/src/Luban/Luban.csproj" -c Release -o Luban
