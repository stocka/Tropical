Imports CommandLine

''' <summary>
''' Contains the different command line options that can be
''' provided to this application.
''' </summary>
Public Class CommandLineOptions

  ''' <summary>
  ''' Gets or sets the path to the directory that will be scanned for sprite images.
  ''' </summary>
  ''' <value>
  ''' The path to the directory that will be scanned for sprite images.
  ''' </value>
  <[Option]("d"c, "directory", DefaultValue:=".",
    HelpText:="The path to the directory that will be scanned for sprite images.")>
  Public Property SourceDirectory() As String

  ''' <summary>
  ''' Gets or sets the path where the sprite sheet project file will be saved.
  ''' </summary>
  ''' <value>
  ''' The path where the sprite sheet project file will be saved.
  ''' </value>
  <[Option]("f"c, "file",
    HelpText:="The path where the sprite sheet project file will be saved. Will default to the name of the containing directory.")>
  Public Property DestinationPath() As String

  ''' <summary>
  ''' Gets or sets the name of the CSS class that will be used for all sprites.
  ''' </summary>
  ''' <value>
  ''' The name of the CSS class that will be used for all sprites.
  ''' </value>
  <[Option]("c"c, "class",
    HelpText:="The name of the CSS class that will be used for all sprites. Will default to the name of the containing directory.")>
  Public Property BaseClassName() As String

  ''' <summary>
  ''' Gets or sets the base file name that will be used
  ''' to generate the sprite sheet and associated CSS stylesheet.
  ''' </summary>
  ''' <value>
  ''' The base file name that will be used to generate the
  ''' sprite sheet and associated CSS stylesheet.
  ''' </value>
  <[Option]("b"c, "basefilename",
    HelpText:="The base file name that will be used to generate the sprite sheet and associated CSS stylesheet. Will default to the name of the containing directory.")>
  Public Property BaseFileName() As String

  ''' <summary>
  ''' Gets or sets the height, in pixels, of each image.
  ''' </summary>
  ''' <value>
  ''' The height, in pixels, of each image.
  ''' </value>
  <[Option]("h"c, "height", DefaultValue:=16,
    HelpText:="The height, in pixels, of each sprite image.")>
  Public Property ImageHeight() As Int32

  ''' <summary>
  ''' Gets or sets the width, in pixels, of each image.
  ''' </summary>
  ''' <value>
  ''' The width, in pixels, of each image.
  ''' </value>
  <[Option]("w"c, "width", DefaultValue:=16,
    HelpText:="The width, in pixels, of each sprite image.")>
  Public Property ImageWidth() As Int32

  ''' <summary>
  ''' Gets or sets the set of file extensions from which
  ''' images will be generated.
  ''' </summary>
  ''' <value>
  ''' The set of file extensions from which images will be generated.
  ''' </value>
  <[Option]("e"c, "extensions", DefaultValue:="jpg, jpeg, gif, png",
    HelpText:="The set of file extensions from which images will be generated.")>
  Public Property FileExtensions() As String

  ''' <summary>
  ''' Gets or sets the search pattern that will be used
  ''' to retrieve files in the directory.
  ''' </summary>
  ''' <value>
  ''' The search pattern that will be used to retrieve
  ''' files in the directory.
  ''' </value>
  <[Option]("p"c, "pattern", DefaultValue:="*",
    HelpText:="The search pattern that will be used to retrieve files in the directory.")>
  Public Property FileSearchPattern() As String

  ''' <summary>
  ''' Gets or sets the delimiters that will be used to
  ''' separate out CSS class names.
  ''' </summary>
  ''' <value>
  ''' The delimiters that will be used to separate out CSS class
  ''' names.
  ''' </value>
  <[Option]("delimiters", DefaultValue:="_- ",
    HelpText:="The delimiters that will be used to separate out CSS class names.")>
  Public Property ClassDelimiters() As String

  ''' <summary>
  ''' Gets or sets the collection of CSS class names
  ''' that will be interpreted as hover classes.
  ''' </summary>
  ''' <value>
  ''' The collection of CSS class names that will be
  ''' interpreted as hover classes.
  ''' </value>
  <[Option]("hoverclasses",
    HelpText:="The collection of CSS class names that will be interpreted as ""hover"" classes.")>
  Public Property HoverClassNames() As String

  ''' <summary>
  ''' Gets or sets the collection of CSS class names
  ''' that will be interpreted as filter classes.
  ''' </summary>
  ''' <value>
  ''' The collection of CSS class names that will be
  ''' interpreted as filter classes.
  ''' </value>
  <[Option]("filterclasses",
    HelpText:="The collection of CSS class names that will be interpreted as ""filter"" classes.")>
  Public Property FilterClassNames() As String

  ''' <summary>
  ''' Gets or sets the logging level to use.
  ''' </summary>
  ''' <value>
  ''' The logging level to use.
  ''' </value>
  <[Option]("l"c, "level", DefaultValue:=Controllers.CommandLineLogger.LogLevel.Information,
    HelpText:="The logging level to use.")>
  Public Property LogLevel() As Controllers.CommandLineLogger.LogLevel

  ''' <summary>
  ''' Gets the help text for the application.
  ''' </summary>
  ''' <returns>The help text for the application.</returns>
  <HelpOption()>
  Public Function GetHelpText() As String

    Dim helpBuilder As New Text.HelpText()

    With helpBuilder
      .Heading = New Text.HeadingInfo("Tropical.GeneratorCommandLine")
      .Copyright = New Text.CopyrightInfo("Andrew Stock", 2013)
      .AdditionalNewLineAfterOption = True
      .AddDashesToOption = True
    End With

    helpBuilder.AddPreOptionsLine("Tropical.GeneratorCommandLine takes the contents of an image directory and uses it to generate an equivalent sprite sheet project file.")
    helpBuilder.AddPreOptionsLine("")
    helpBuilder.AddPreOptionsLine("Usage: Tropical.GeneratorCommandLine -d <source directory>")

    ' Add options
    helpBuilder.AddOptions(Me)

    Return helpBuilder.ToString()

  End Function

End Class
