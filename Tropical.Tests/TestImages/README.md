# TestImages Readme

All new images should be added to the Tropical.Tests project and marked as "Copy if newer" for the "Copy to Output" directory.  When the tests execute, it is dependent upon these images being located in the "TestIcons" subdirectory of the current (test assembly) directory.

In addition, if the image is the standard 16x16 size, and is of a format not already covered, it should be included in the SpriteSheetGenerator.TestAllImageFormats test method.