Imports System.IO
Imports System.ComponentModel.DataAnnotations
Imports cl = CommandLine
Imports Tropical.Models
Imports Tropical.Controllers

Module TropicalGeneratorCommandLine

  Private ReadOnly Logger As New CommandLineLogger()

  Sub Main()

    Dim tropicalArgs As New CommandLineOptions()

    ' Parse out the args.
    If cl.Parser.Default.ParseArguments(Environment.GetCommandLineArgs(), tropicalArgs) Then

      ' Copy over our log level
      Logger.Level = tropicalArgs.LogLevel

      Try

        ' Validate the source directory before we do anything else.
        If Not Validation.ValidateDirectoryPath(tropicalArgs.SourceDirectory) Then
          LogErrorAndExit("The specified source directory is invalid.")
        End If

        ' Convert the source directory to an absolute path and get its
        ' name, which we'll use for defaults.
        Dim fullSourcePath As String = Path.GetFullPath(tropicalArgs.SourceDirectory)
        Dim sourceDirectoryName As String = Path.GetFileName(fullSourcePath)

        ' Now make sure the directory actually exists.
        If Not System.IO.Directory.Exists(fullSourcePath) Then
          LogErrorAndExit("The specified source directory does not exist.")
        End If

        ' Set some defaults if we don't have values provided.
        If String.IsNullOrWhiteSpace(tropicalArgs.BaseClassName) Then
          ' If we don't have a base CSS class, use the name of the source directory.
          tropicalArgs.BaseClassName = sourceDirectoryName
          Logger.Information("A base CSS class name was not specified, so a default of """ & tropicalArgs.BaseClassName & """ will be used instead." & Environment.NewLine)
        End If

        If String.IsNullOrWhiteSpace(tropicalArgs.BaseFileName) Then
          ' If we don't have a base file name, use the name of the source directory.
          tropicalArgs.BaseFileName = sourceDirectoryName
          Logger.Information("A base file name was not specified, so a default of """ & tropicalArgs.BaseFileName & """ will be used instead." & Environment.NewLine)
        End If

        If String.IsNullOrWhiteSpace(tropicalArgs.DestinationPath) Then
          ' If we don't have a destination path, put it in the source directory
          ' with the extension ".sprites.xml".
          tropicalArgs.DestinationPath = Path.Combine(fullSourcePath, sourceDirectoryName & ".sprites.xml")
          Logger.Information("A destination file path was not specified, so a default of """ & tropicalArgs.DestinationPath & """ will be used instead." & Environment.NewLine)
        End If

        ' Validate the destination path.
        If Not Validation.ValidateFilePath(tropicalArgs.DestinationPath) Then
          LogErrorAndExit("The specified destination file path is invalid.")
        End If

        ' Now generate the equivalent generator options.
        Dim options As SpriteSheetContentGeneratorOptions = BuildContentGeneratorOptions(tropicalArgs)

        ' Validate those options.
        ValidateGeneratorOptions(options)

        ' Now we have our generator options.  Attempt to actually generate something!
        Dim generator As New SpriteSheetContentGenerator(options)
        generator.Logger = Logger

        Dim sheet As SpriteSheet = generator.Generate(fullSourcePath)

        ' Make sure we got something back.
        If sheet Is Nothing Then
          LogErrorAndExit("The sprite sheet could not be generated.")
        End If

        ' Now save the sprite sheet.
        If SpriteSheetFileUtilities.SaveSpriteSheet(tropicalArgs.DestinationPath,
                                                    sheet,
                                                    Logger) Then
          Logger.Information("Sprite sheet saved to """ & tropicalArgs.DestinationPath & """.")
          Environment.ExitCode = 0
        Else
          LogErrorAndExit("The sprite sheet was generated, but could not be saved.")
        End If

      Catch ex As Exception
        LogErrorAndExit("An error was encountered attempting to generate the sprite sheet project file.", exception:=ex)
      End Try

    End If

  End Sub

  ''' <summary>
  ''' Validates the content generator options. If invalid,
  ''' an error message will be displayed and the application will exit.
  ''' </summary>
  ''' <param name="options">The generator options to validate.</param>
  Private Sub ValidateGeneratorOptions(options As SpriteSheetContentGeneratorOptions)

    Dim validationContext As New ValidationContext(options)
    Dim results As New List(Of ValidationResult)

    ' Let's try to validate this.
    If Not Validator.TryValidateObject(options, validationContext, results, True) Then
      ' It's not valid, so we're going to build an error message from our results and exit out.
      Logger.Error("The specified configuration is invalid:" & Environment.NewLine &
                   Validation.GetValidationResultMessage(results, False))
      Environment.Exit(-1)
    End If

  End Sub

  ''' <summary>
  ''' Constructs the equivalent 
  ''' <see cref="SpriteSheetContentGeneratorOptions">content generator options</see>
  ''' from the provided command-line options.
  ''' </summary>
  ''' <param name="commandOpts">The command-line options.</param>
  ''' <returns>An instantiated set of content generator options.</returns>
  Private Function BuildContentGeneratorOptions(commandOpts As CommandLineOptions) As SpriteSheetContentGeneratorOptions

    Dim opts As New SpriteSheetContentGeneratorOptions()

    With opts
      .BaseClassName = commandOpts.BaseClassName
      .BaseFileName = commandOpts.BaseFileName
      .FileSearchPattern = commandOpts.FileSearchPattern
      .ImageHeight = commandOpts.ImageHeight
      .ImageWidth = commandOpts.ImageWidth

      ' Split out our "difficult" properties into arrays
      .ClassDelimiters = commandOpts.ClassDelimiters.ToArray()
      .FileExtensions = SplitDelimitedString(commandOpts.FileExtensions)
      .FilterClassNames = SplitDelimitedString(commandOpts.FilterClassNames)
      .HoverClassNames = SplitDelimitedString(commandOpts.HoverClassNames)
    End With

    Return opts

  End Function

  ''' <summary>
  ''' Splits a delimited string into its constituent elements.
  ''' </summary>
  ''' <param name="delimitedString">The delimited string.</param>
  ''' <returns>The results of the split.</returns>
  Private Function SplitDelimitedString(delimitedString As String) As String()

    ' Handle null/empty (not necessarily whitespace!) strings
    If String.IsNullOrEmpty(delimitedString) Then
      Return {}
    End If

    ' Split on spaces, tabs, CR, LF, semicolons, and commas
    Return delimitedString.Split({" "c, ChrW(&H9), ChrW(&HA), ChrW(&HD), ";"c, ","c},
                                 StringSplitOptions.RemoveEmptyEntries)

  End Function

  Private Sub LogErrorAndExit(message As String, Optional exception As Exception = Nothing)
    Logger.Error(message, exception:=exception)
    Environment.Exit(-1)
  End Sub

End Module
