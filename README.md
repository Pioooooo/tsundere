# Tsundere

A model checker written in C#. Implementation detail is at [here](implementation.md).

CS3959 assignment, ACM Class, SJTU

# Build

1. Install [.NET](https://dotnet.microsoft.com/download)
2. `cd Tsundere && dotnet publish -c Release -r <RID> --self-contained false -p:PublishReadyToRun=true`

# Usage

```
tsundere [options]

Options:
  -t, --ts <ts>          The transition system file to read from. [default: TS.txt]
  -b, --bench <bench>    The LTL formula benchmark file to read from. [default: benchmark.txt]
  -o, --output <output>  The file to write to. [default: stdout]
  -v, --verbose          Output verbose information.
  --version              Show version information
  -?, -h, --help         Show help and usage information
```
