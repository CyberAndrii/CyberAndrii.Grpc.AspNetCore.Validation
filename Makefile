TAG := $(shell git describe --tags `git rev-list --tags --max-count=1` || echo 0.0.0)
HASH := $(shell git rev-parse --short HEAD)
VERSION := $(TAG)+$(HASH)

build:
	dotnet build \
		-c Release \
		-p:GeneratePackageOnBuild=true \
		-p:GenerateNugetPackage=true \
		-p:Version=$(VERSION) \
		-p:InformationalVersion=$(VERSION) \
		-p:PackageVersion=$(VERSION) \
		-p:RepositoryUrl="$(REPOSITORY_URL)"

push:
ifndef KEY
	$(error NuGet API key is not set)
endif
	$(eval SOURCE ?= https://api.nuget.org/v3/index.json)
	dotnet nuget push */src/*/bin/Release/*.$(TAG).nupkg \
		--skip-duplicate \
		--api-key "$(KEY)" \
		--source "$(SOURCE)"
