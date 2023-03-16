#!/bin/bash
export VERSION=5.0.9
export PATCH=criteo5
mkdir dotnet-runtime-${VERSION}-${PATCH}-linux-x64
cd dotnet-runtime-${VERSION}-${PATCH}-linux-x64
tar -xzf ../dotnet-runtime-${VERSION}-${PATCH}-linux-x64.tar.gz
cd shared/Microsoft.NETCore.App/${VERSION}-${PATCH}
tar -xzf ../../../../dotnet-crossgen2-${VERSION}-${PATCH}-linux-x64.tar.gz
tar -xzf ../../../../dotnet-runtime-symbols-${VERSION}-${PATCH}-linux-x64.tar.gz
cd ../../..
tar -zcf ../dotnet-runtime-${VERSION}-${PATCH}-linux-x64-packed.tar.gz .
cd ..
ls -la dotnet-runtime-${VERSION}-${PATCH}-linux-x64-packed.tar.gz
