Imports cl = CommandLine
Imports Tropical.Models
Imports Tropical.Controllers

Module TropicalCommandLine

  Private ReadOnly Logger As New CommandLineLogger()

  Sub Main()

    Dim tropicalArgs As New CommandLineOptions()

    ' Parse out the args.
    If cl.Parser.Default.ParseArguments(Environment.GetCommandLineArgs(), tropicalArgs) Then

      ' Copy over our log level
      Logger.Level = tropicalArgs.LogLevel

      Try

        ' Validate the destination path.
        If Not Validation.ValidateDirectoryPath(tropicalArgs.DestinationPath) Then
          LogErrorAndExit("The specified destination path is invalid.")
        End If

        ' Validate that the sprite sheet path is valid and it exists.
        If Not Validation.ValidateFilePath(tropicalArgs.SpriteSheetFile) Then
          LogErrorAndExit("The path to the sprite sheet file is invalid.")
        End If

        If Not System.IO.File.Exists(tropicalArgs.SpriteSheetFile) Then
          LogErrorAndExit("The specified sprite sheet file does not exist.")
        End If

        ' If a custom image path is provided, validate that as well.
        If Not String.IsNullOrWhiteSpace(tropicalArgs.CustomImagePath) Then

          If Not Validation.ValidateDirectoryPath(tropicalArgs.CustomImagePath) Then
            LogErrorAndExit("The specified custom image path is invalid.")
          End If

          If Not IO.Directory.Exists(tropicalArgs.CustomImagePath) Then
            LogErrorAndExit("The specified custom image path refers to a nonexistent directory.")
          End If

        End If

        ' Get our sprite sheet
        Dim sheet As SpriteSheet = SpriteSheetFileUtilities.LoadSpriteSheet(tropicalArgs.SpriteSheetFile, Logger)

        ' If it failed, error out.
        If sheet Is Nothing Then
          Environment.Exit(-1)
        End If

        ' If we have a custom image path, apply that now
        If Not String.IsNullOrWhiteSpace(tropicalArgs.CustomImagePath) Then
          SpriteSheetFileUtilities.RelocateImagePaths(sheet, tropicalArgs.CustomImagePath)
        End If

        ' Now set up the generator with our logger.
        Dim generator As New SpriteSheetGenerator(sheet)
        generator.Logger = Logger

        ' Generate the sheet.
        If generator.Generate(tropicalArgs.DestinationPath) Then
          Logger.Information("Sprite sheet generated.")
          Environment.ExitCode = 0
        Else
          LogErrorAndExit("One or more errors were encountered attempting to generate the sprite sheet.")
        End If

      Catch ex As Exception
        LogErrorAndExit("An error was encountered attempting to generate the sprite sheet.", exception:=ex)
      End Try

    End If

  End Sub

  Private Sub LogErrorAndExit(message As String, Optional exception As Exception = Nothing)
    Logger.Error(message, exception:=exception)
    Environment.Exit(-1)
  End Sub

End Module
