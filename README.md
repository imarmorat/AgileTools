## Why this project

Jira is used in many companies I worked for; while this provides some value in terms of agile management, this tool is fairly bad at displaying information the way you want.
Initially build as a ticket management tool, it grew popularity in the agile community when the Agile plugin arrived.

## Important

It is in early stage and massive refactoring are frequent.

## Roadmap

- Finalizing current analysers
- Strengthening code and error handling
- Graph: using libs such as D3.js, offering some out-of-the-box common graphs
- Execution command line (as opposed to current interpreter)

## What does it do

A tool should adapt to the need of the ones using it rather than the opposite; many agile tools these days still struggle to offer such flexibility.
To solve this issue, this tool allows for data extraction (currently only supporting JIRA) and analysis such as:
- Cumulative flow
- Product backlog analysis (defined by a set of rules that can easily be extended)

It allows for macro so as to speed things up; the "macro" command has the following parameters:

| parameter | description |
| ---------- | ------------ |
| list | list all available macros |
| peek (macro name) | displays the content of a macro |
| status | displays the current status (i.e. whether this is recording or not) |
| record (macro name) | start recording a macro; you will need to use "save" to persist it |
| save | persist the macro (into a file in the execution directory |
| cancel | stop recording and macro discarded |
| run (macro name) | run a specific macro |
| delete (macro name) | remove a macro (deletes the associated file) |

