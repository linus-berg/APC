# About
The git ACM processor produces git bundle files to incrementally update repositories.

# How to use git bundles

The first git bundle will be named
`<repository>@00010101000000-<xxx (utc.now)>.bundle`
and the next increment of the bundle will be named as
`<repository>@<xxx>-<yyy (utc.now)>`

The approach I use is as following
```console
# Rename the initial bundle to the repository name
mv <repository>@00010101000000-<xxx>.bundle <repository>

# This creates a mirror for serving via http
git clone --bare <repository>

# Move the initial bundle to a applied folder and rename to original name
mv <repository> /storage/applied/<repository>@00010101000000-<xxx>.bundle
```


```ini
# Edit the $GIT_DIR/config to include
[remote "origin"]
  url = path/to/bundle
  fetch = +refs/*:refs/*
```
```console
# This should work for the above.
sed '/url = */a\\tfetch = +refs/*:refs/*' config
```

```console
# If serving over http
git update-server-info
```


## To apply the next incremental updates updates do the following:
```console
# Before applying the next update, always verify if it can be applied first!
cd <repository>

# If it is missing previous refs it will exit with non 0 code.
git bundle verify ../<repository>@<xxx>-<yyy>.bundle

cd ..

# Rename the incremental update bundle to.
# NOTE: It is important this bundle is located at the same place as the intial!
mv <repository@<xxx>-<yyy>.bundle <repository>

cd <repository>

git fetch --all

# If serving over http
git update-server-info

cd ..
# Move the incremental bundle to the applied folder and rename to the original name
mv <repository> /storage/applied/<repository>@<xxx>-<yyy>.bundle
```

