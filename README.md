# Apex Tools (C#)
A library to extract and repack [Avalanche Studios](https://avalanchestudios.com/) game files.

Supported for all Just Cause games is planned, with other Avalanche titles intended for support if all goes well.

## Game support status
|             Studio             |        Game       |         Engine        | Status | Notes |
| ------------------------------ | ----------------- | --------------------- | ------ | ----- |
| Avalanche Studios              | Just Cause 2      | Avalanche 2.0 Engine  | TBD    | N/A   |
| Avalanche Studios              | Just Cause 3      | Avalanche 3.0 Engine? | WIP    | N/A   |
| Avalanche Studios              | Just Cause 4      | Apex Game Engine      | WIP    | N/A   |
| Ava Studio / Systemic Reaction | Second Extinction | Apex Game Engine      | TBD    | N/A   |
| Ava Studio / Systemic Reaction | Generation Zero   | Apex Game Engine?     | TBD    | N/A   |
| Avalanche Studios              | Rage 2            | Apex Game Engine      | TBD    | N/A   |

### File type status
|     Game     |  File type  | Version |         Status        |
| ------------ | ----------- | ------- | --------------------- |
| Just Cause 3 | IRTPC       | 1       | Completed (Perfect)   |
|              | RTPC        | 1       | Completed (Perfect)   |
|              | AAF         | 1       | Completed (Imperfect) |
|              | SARC        | 2       | Completed (Perfect)   |
|              | ADF         | 4       | WIP                   |
|              | TAB / ARC   | X       | ToDo                  |
|              | DDSC / AVTX | X       | ToDo                  |

## Releases
- Initial release planned once full Just Cause 3 support is achieved

*Note*: Feel free to build from these tools in the mean time.

## External dependencies
- None so far

*Note*: Just Cause 4 (and potentially other Avalanche games) uses Oodle (`oo2core_7_win64.dll`) to de/compress TAB/ARC.
Once support for Just Cause 4 is rolled out, this will be required to process those files.

I am unable to share this file due to licensing, but if you have the game it should be located in your installation folder.

## Discord
Feel free to join the EonZeNx server here: https://discord.gg/SAjVFmMGdd

# References

- **[aaronkirkham's jc-model-renderer](https://github.com/aaronkirkham)**: The JC Model Renderer had some very useful 
  info, especially regarding compression.
- **[kk19's DECA](https://github.com/kk49/deca)**: Very helpful for better detailed file formats.

# Disclaimer
All product names, logos, and brands are property of their respective owners. All company, product and service names
used in this website are for identification purposes only. Use of these names, logos, and brands does not imply endorsement.
