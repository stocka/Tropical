Imports CommandLine

''' <summary>
''' Contains the different command line options that can be
''' provided to this application.
''' </summary>
Public Class CommandLineOptions

  ''' <summary>
  ''' Gets or sets the destination folder for the sprite sheet.
  ''' </summary>
  ''' <value>
  ''' The destination folder for the sprite sheet.
  ''' </value>
  <[Option]("d"c, "destination", Required:=True,
    HelpText:="The destination folder for the sprite sheet.")>
  Public Property DestinationPath() As String

  ''' <summary>
  ''' Gets or sets the path to the sprite sheet file.
  ''' </summary>
  ''' <value>
  ''' The path to the sprite sheet file.
  ''' </value>
  <[Option]("f"c, "file", Required:=True,
    HelpText:="The path to the source sprite sheet file.")>
  Public Property SpriteSheetFile() As String

  ''' <summary>
  ''' Gets or sets the logging level to use.
  ''' </summary>
  ''' <value>
  ''' The logging level to use.
  ''' </value>
  <[Option]("l"c, "level", DefaultValue:=CommandLineLogger.LogLevel.Information,
    HelpText:="The logging level to use.")>
  Public Property LogLevel() As CommandLineLogger.LogLevel

  ''' <summary>
  ''' Gets or sets the custom path to the folder containing all sprite sheet images.
  ''' </summary>
  ''' <value>
  ''' The custom path to the folder containing all sprite sheet images.
  ''' </value>
  <[Option]("i"c, "imagelocation", HelpText:="The custom path to the folder containing all source images.")>
  Public Property CustomImagePath() As String = String.Empty

  ''' <summary>
  ''' Gets the help text for the application.
  ''' </summary>
  ''' <returns>The help text for the application.</returns>
  <HelpOption()>
  Public Function GetHelpText() As String

    Dim helpBuilder As New Text.HelpText()

    With helpBuilder
      .Heading = New Text.HeadingInfo("Tropical.CommandLine")
      .Copyright = New Text.CopyrightInfo("Andrew Stock", 2013)
      .AdditionalNewLineAfterOption = True
      .AddDashesToOption = True
    End With

    helpBuilder.AddPreOptionsLine("Usage: Tropical.CommandLine -f <sprite sheet file> -d <destination folder>")

    ' Add options
    helpBuilder.AddOptions(Me)

    Return helpBuilder.ToString()

  End Function

End Class
