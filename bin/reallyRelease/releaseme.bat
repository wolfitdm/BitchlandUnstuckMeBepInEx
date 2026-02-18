@ECHO OFF
setlocal enabledelayedexpansion
for %%f in (*.zip) do (
  echo Y | gh release delete-asset v2.0.0 %%f
  echo Y | gh release upload v2.0.0 %%f
)
for %%f in (*.json) do (
  echo Y | gh release delete-asset v2.0.0 %%f
  echo Y | gh release upload v2.0.0 %%f
)