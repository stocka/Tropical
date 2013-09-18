Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class SpriteSheetGeneratorTests

  <TestMethod()>
  Public Sub TestEmpty()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
    End With

    Dim generator As New Controllers.SpriteSheetGenerator(sheet)

    Assert.AreEqual(0, generator.SpriteCount)
    Assert.AreEqual(0, generator.Dimensions.Height)
    Assert.AreEqual(0, generator.Dimensions.Width)

    generator.Generate()

  End Sub

End Class