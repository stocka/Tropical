Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class SpriteSheetGeneratorTests

  Private Log As TestLogger

  <TestInitialize()>
  Public Sub Initialize()
    Me.Log = New TestLogger()
  End Sub

  ''' <summary>
  ''' Gets a <see cref="Controllers.SpriteSheetGenerator" /> instance
  ''' for the specified sprite sheet, initialized with properties
  ''' such as this instance's logger.
  ''' </summary>
  ''' <param name="sheet">The sprite sheet.</param>
  ''' <returns>The initialized generator.</returns>
  Private Function GetGenerator(sheet As Models.SpriteSheet) As Controllers.SpriteSheetGenerator

    Dim generator As New Controllers.SpriteSheetGenerator(sheet)

    ' Set the logger
    generator.Logger = Log

    Return generator

  End Function

  <TestMethod()>
  Public Sub TestEmpty()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
    End With

    Dim generator = GetGenerator(sheet)

    Assert.AreEqual(0, generator.SpriteCount)
    Assert.AreEqual(0, generator.Dimensions.Height)
    Assert.AreEqual(0, generator.Dimensions.Width)

    ' Make sure generation doesn't succeed
    Assert.IsFalse(generator.Generate(GetDestinationPath()))

    ' Make sure an error log entry was created.
    Assert.AreEqual(1, Log.ErrorEntries.Count)
    Assert.AreEqual("No sprites have been added to the sheet.", Log.ErrorEntries.First().Message)

  End Sub

  Private Function GetDestinationPath()
    Return System.IO.Directory.GetCurrentDirectory()
  End Function

End Class