#thank you chatgpt🤑

import os
import shutil

src = input("Source folder: ").strip()
dst = input("Destination folder: ").strip()

if input("Continue? (y/n): ").lower() != "y":
    print("Cancelled.")
    exit()

print("\nClearing destination")

for entry in os.scandir(dst):
    try:
        if entry.is_file() or entry.is_symlink():
            os.unlink(entry.path)
        else:
            shutil.rmtree(entry.path)
    except Exception as e:
        print(f"Delete failed: {entry.path} ({e})")

print("\nMoving files & folders")

for entry in os.scandir(src):
    dst_path = os.path.join(dst, entry.name)
    try:
        if entry.is_file():
            shutil.copy2(entry.path, dst_path)
        else:
            shutil.copytree(entry.path, dst_path)
        print(f"Copied: {entry.name}")
    except Exception as e:
        print(f"Copy failed: {entry.name} ({e})")

print("\nDone!")