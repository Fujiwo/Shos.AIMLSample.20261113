# English README Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add `README.en.md` as a natural English version of the expanded Japanese README.

**Architecture:** Create one Markdown file at the repository root, translating the approved expanded README from branch `docs/japanese-readme` (commit `2ec705f`). Preserve its sections, factual scope, commands, paths, identifiers, data labels, and example structure; do not change the Japanese README or source code.

**Tech Stack:** Markdown; .NET 10, PowerShell, Python, Matplotlib, WPF command examples.

---

### Task 1: Create the English README

**Files:**
- Create: `README.en.md`
- Reference: `README.md` from branch `docs/japanese-readme` at commit `2ec705f`
- Reference: `docs/superpowers/specs/2026-11-11-english-readme-design.md`

- [ ] Translate all sections in the source README in the same order: repository overview, key points, sample table, prerequisites, build and test, running each sample, directory structure, and learning notes.
- [ ] Use clear, natural English suitable for beginners; preserve the meaning without adding claims about accuracy, outputs, data provenance, or guarantees.
- [ ] Keep project names, relative paths, commands, pixel dimensions, and Python label values unchanged. Preserve all PowerShell command blocks, including `Push-Location` and `Pop-Location` around the Python script command.
- [ ] Keep `README.md` and every source/configuration file unchanged; do not add a language-switch link.

### Task 2: Verify content parity and Markdown

**Files:**
- Verify: `README.en.md`
- Compare with: `README.md` from branch `docs/japanese-readme` at commit `2ec705f`

- [ ] Confirm every source section is represented once and in the same order in `README.en.md`.
- [ ] Extract the fenced PowerShell command blocks from both READMEs and confirm they match exactly:

```powershell
dotnet build .\Shos.AIMLSample.20261113.sln --configuration Debug
dotnet test .\NeuralNetwork.Core.Tests\NeuralNetwork.Core.Tests.csproj
dotnet run --project .\Mnist.CSharp\Mnist.CSharp.csproj --configuration Debug
dotnet run --project .\NeuralNetworkSample.WPF\NeuralNetworkSample.WPF.csproj --configuration Debug
python -m pip install matplotlib
Push-Location .\NeuralNetwork.Python
python .\classification.py
Pop-Location
```

- [ ] Confirm paths, project names, and label values `0` and `1` are unchanged; confirm all Markdown code fences are balanced.
- [ ] Run `git diff --check` and confirm only `README.en.md` is staged for the implementation commit.

### Task 3: Commit the English README

**Files:**
- Commit: `README.en.md`

- [ ] Stage only `README.en.md` with `git add -- README.en.md`.
- [ ] Commit with this message and required trailer:

```text
docs: add English README

Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>
```
