# Repository Splitter
Repositories can get large with multiple projects, especially when they do not follow the convention of one project per repository. `Repository Splitter` splits a target repository using available strategies.

The application uses [CommandLineParser](https://www.nuget.org/packages/commandlineparser) to help with parsing command line arguments.

## Split Strategies
Although there is only one strategy at this time, additional strategies can be introduced in the future.

### Specified Subdirectories
This split strategy creates a new repository from content and history extracted from specified subdirectories in the repository. Any unrelated content and history are excluded from the new repository.

At this time, the target repository is not changed.

```
RepositorySplitter.exe

-r                  Required. Directory path for the repository to split.

-s                  Required. The name of the new repository that results
                    from the splitting.

-d                  Required. Names of directories to include in the new
                    repository, separated by a space. Directories are
                    expected to be relative paths from the repository root.

--subdirectories    Whether the directories to include are at the subdirectory
                    level.
```

#### Example
Given the following repo folder structure
```
targetRepo
    directory1
	    project1.csproj
	directory2
	    project2.csproj
	directory3
	    project3.csproj
	solution.sln
```
The following command would create `newRepo` as a sibling directory to `targetRepo` with only `directory1` and `directory2` content and history. `newRepo` would not have any content or history from `directory3`.
```
RepositorySplitter.exe -r targetRepo -s newRepo -d "directory1 directory2"
```
The folder structure for `newRepo` would be
```
newRepo
    directory1
	    project1.csproj
	directory2
	    project2.csproj
	solution.sln
```

For large repositories that need to split into multiple smaller repositories, one can execute something like the following (e.g. in a BAT file)
```
RepositorySplitter.exe -r targetRepo -s newRepo -d "directory1 directory2"
RepositorySplitter.exe -r targetRepo -s newRepo2 -d directory 3
```

## Next Steps

### Add Upstream Remotes
Repositories created by `Repository Splitter` do not have any upstream remote repository associated. This is intentional as to allow users to associate their own remote associations after splitting.

### Fix Project References
When splitting a repository whose projects reference one another, project references will likely break and would need to be adjusted in order to compile. One way to do this is to update projects in one repository to reference packaged (e.g. using [NuGet](https://www.nuget.org)) project assemblies published from another repository.

Using the example `targetRepo` above, say `directory1/project1` referenced `directory3/project3`, but now each project has been split into separate repositories. One can [package `directory3/project3`](https://docs.nuget.org/create/creating-and-publishing-a-package), and even [host it on a personal NuGet gallery feed](https://docs.nuget.org/create/hosting-your-own-nuget-feeds). Then `directory1/project1` can install the packaged `directory3/project3`.

This approach may one day be supported by `Repository Splitter` to automatically package referenced project assembles and update project references.