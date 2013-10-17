''' <summary>
''' Contains information about a sprite sheet
''' that has been saved to or loaded from a file.
''' </summary>
Public Class SpriteSheetFileInformation

  ''' <summary>
  ''' Gets or sets the sprite sheet that was saved or loaded.
  ''' </summary>
  ''' <value>
  ''' The sprite sheet that was saved or loaded.
  ''' </value>
  Public Property SpriteSheet() As Models.SpriteSheet

  ''' <summary>
  ''' Gets or sets the path to the source or destination file.
  ''' If an entirely new sprite sheet is being generated, this
  ''' value can be blank.
  ''' </summary>
  ''' <value>
  ''' The path to the source or destination file.
  ''' </value>
  Public Property FilePath() As String

  ''' <summary>
  ''' Gets or sets the name of the source or destination file.
  ''' If an entirely new sprite sheet is being generated, this
  ''' value can be blank.
  ''' </summary>
  ''' <value>
  ''' The name of the source or destination file.
  ''' </value>
  Public ReadOnly Property FileName() As String
    Get

      ' Get the name from the full path if defined
      If Not String.IsNullOrWhiteSpace(Me.FilePath) Then
        Return System.IO.Path.GetFileName(Me.FilePath)
      Else
        Return String.Empty
      End If

    End Get
  End Property

  ''' <summary>
  ''' Initializes a new instance of the <see cref="SpriteSheetFileInformation"/> class.
  ''' </summary>
  ''' <param name="sheet">The sprite sheet that was saved or loaded.</param>
  ''' <param name="filePath">The path to the source or destination file.</param>
  Public Sub New(sheet As Models.SpriteSheet, filePath As String)
    Me.SpriteSheet = sheet
    Me.FilePath = filePath
  End Sub

End Class
