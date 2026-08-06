---
name: Imported project setup
description: Environment-specific setup lessons for this imported multi-project repository.
---

The frontend is the Replit-visible application and lives in a subdirectory; dependency installation commands must run from `frontend/`, not the repository root.

**Why:** Installing from the root creates an unrelated package manifest and can hide the actual app dependency state.

**How to apply:** Use the existing `frontend/package.json`, run package scripts from `frontend/`, and keep the backend as a separate .NET project unless the user explicitly asks for full-stack runtime setup.