import os
import re

token_regex = re.compile(r'^\s*//.*Token:')

start_dir = os.getcwd()

folders = []
for root, dirs, files in os.walk(start_dir):
    folders.append(root)

print("\nFolders to process")
for f in folders:
    print(f)

confirm = input("Continue? (y/n): ").lower()
if confirm != "y":
    print("Cancelled.")
    exit()

print("\nProcessing files")

for root, dirs, files in os.walk(start_dir):
    for file in files:
        if file.endswith(".cs"):
            path = os.path.join(root, file)

            with open(path, "r", encoding="utf-8", errors="ignore") as f:
                lines = f.readlines()

            new_lines = [line for line in lines if not token_regex.search(line)]

            if len(new_lines) != len(lines):
                with open(path, "w", encoding="utf-8", errors="ignore") as f:
                    f.writelines(new_lines)
                print(f"Cleaned: {path}")

print("\nDone!")
