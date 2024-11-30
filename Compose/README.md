# Development infrastructure environment
The `docker-compose.yml` in this directory will set up the surrounding
infrastructure environment for development, or at least hopefully it does.

## Run with `docker`
Make sure that `docker` and `docker-compose` is installed on your machine. Then
start the development environment with:

```console
sudo docker-compose up
```

## Run with `podman` in rootless mode on Fedora
The development environment can be run with `podman` in rootless mode on Fedora
as well.

### Install and prepare
Install `podman` with docker compatibility:

```console
sudo dnf install podman podman-docker docker-compose
```

> [!WARNING]
> The above command assumes that you have NOT installed any `docker` RPMs from
> outside the Fedora base repository. If so, you might need to remove them
> before trying this rootless variant.

> [!NOTE]
> The package `docker-compose` referenced above is the one present in the
> standard Fedora repository.

Then enable the `podman` service in "user mode":

```console
systemctl --user enable podman.socket
systemctl --user start podman.socket
```

and finally setup the docker environment variable to point to your local
`podman` service:

```console
export DOCKER_HOST=unix:///run/user/$UID/podman/podman.sock
```

> [!NOTE]
> The export of `DOCKE_HOST` need to be set in the terminal before you start
> `docker-compose`. The `podman` service is persistant though, and will survive
> reboots.

> [!TIP]
> If you add the export of `DOCKER_HOST` to your `~/.bashrc` the rootless
> environment will survive reboots and/or new terminal sessions as well.


### Run `docker-compose` with podman in rootless mode
Now you start the APC development environment with:

```console
docker-compose up
```

> [!NOTE]
> Since you now are running in rootless mode, you should not use `sudo`.

Stop it with `Ctrl-C`.
