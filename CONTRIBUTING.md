## General
- If you want to work on something that is not in the issue list, please discuss it first. We can then decide to create an issue for it.
- Please only create pull requests that solve open issues.

## Branching, Commmiting & Merging
- Commits state the issue the work is done on in the first line of the comment of the commit.
- Commits have descriptive comments if more is fixed than the issue itself.
- When making branches, use the following naming convention [issuenr]\_issue-description-here
- Before merging a branch to master, merge master into the branch first to resolve conflicts, and test the software.
- Only merge a branch to master when the issue is done.
- Preferably squash merge an issue branche so there is a single commit on the master branche containing all changes.
- Don't commit work on master of an issue that is still work in progress.
- A single commit on master where an issue is resolved by is fine.
- Remove the branch when it was merged to master, or no longer required.

## Defintion of Ready
- Issues should describe what value is added when it is done. So what bug is fixed, why a feature is proposed to add value or why technical debt should be solved.
- A description is added (or even a visual mock) for GUI related work.

## Definition of Done
- The work has been merged to master.
- All software compiles.
- >= ~80% unit test coverage of newly added code.
- All tests succeed.
- Nuget packages are reused instead of introducing new package for solving the same issue. So no 3 different packages for serializing to and from JSON for example.
- The software committed adheres to the SOLID principles.
- Code refactored or introduced is unit tested.
- There are no to do's in the code
- The related issue is closed.
- The commits that close an issue are linked in the comment section of the issue, if not done automatically by git.
- Bugs and features are added to the release notes describing what was changed in layman terms as much as possible.
