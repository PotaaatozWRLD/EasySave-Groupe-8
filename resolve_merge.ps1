#!/usr/bin/env powershell
# Script to resolve merge conflicts for documentation files

Set-Location "C:\Users\Axel\Documents\A3\Génie_logiciel\EasySave-Groupe-8-1"

# Add resolved files to the staging area
Write-Host "Adding resolved files..." -ForegroundColor Green
git add Documentation/USER_MANUAL.md Documentation/RELEASE_NOTES_v3.0.md Documentation/TECHNICAL_SUPPORT.md README.md

# Commit the merge resolution
Write-Host "Committing merge resolution..." -ForegroundColor Green
git -c core.editor=true commit --no-edit -m "Resolve merge conflicts: Update documentation for v3.0"

Write-Host "Merge resolved successfully!" -ForegroundColor Green
