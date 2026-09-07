#!/bin/bash
find Assets/_Game/Prefabs -type f -name '*.glb' | while IFS= read -r f
do
    tmp="${f%.glb}.tmp.glb"

    echo "Processing: $f"

    gltf-transform resize "$f" "$tmp" \
        --width 512 \
        --height 512 \
    && mv "$tmp" "$f"
done