Imports System.IO
Imports System.Xml.Serialization

''' <summary>
''' Contains methods for working with saving and loading <see cref="SpriteSheet">sprite sheets</see>.
''' </summary>
Public Class SpriteSheetFileUtilities

  ''' <summary>
  ''' The shared XML serializer used to serialize and deserialize
  ''' sprite sheet files.
  ''' </summary>
  Private Shared ReadOnly SheetSerializer As New XmlSerializer(GetType(SpriteSheet), {GetType(Sprite)})

  ''' <summary>
  ''' Prevents a default instance of the <see cref="SpriteSheetFileUtilities"/> class from being created.
  ''' </summary>
  Private Sub New()
  End Sub

  ''' <summary>
  ''' Saves the sprite sheet to the specified path.
  ''' If this is unsuccessful, any error messages will be written to the
  ''' <paramref name="logger" />.
  ''' </summary>
  ''' <param name="filePath">The file path.</param>
  ''' <param name="sheet">The sprite sheet to save.</param>
  ''' <param name="logger">The logger.</param>
  ''' <returns><c>true</c> if the sprite sheet was saved successfully;
  ''' <c>false</c> otherwise.</returns>
  ''' <exception cref="System.ArgumentNullException">
  ''' <paramref name="filePath" /> is null or whitespace.
  ''' </exception>
  ''' <exception cref="System.ArgumentNullException">
  ''' <paramref name="logger" /> is null.
  ''' </exception>
  ''' <exception cref="System.ArgumentException">
  ''' The specified <paramref name="filePath" /> is an invalid file path.
  ''' </exception>
  ''' <exception cref="System.ArgumentException">
  ''' The specified <paramref name="filePath" /> refers to a directory that does not exist
  ''' and could not be created.
  ''' </exception>
  Public Shared Function SaveSpriteSheet(filePath As String, sheet As SpriteSheet, logger As ILogger) As Boolean

    ' Validate our args
    If String.IsNullOrWhiteSpace(filePath) Then
      Throw New ArgumentNullException("filePath")
    End If

    If sheet Is Nothing Then
      Throw New ArgumentNullException("sheet")
    End If

    If logger Is Nothing Then
      Throw New ArgumentNullException("logger")
    End If

    ' Validate the file path
    If Not Validation.ValidateFilePath(filePath) Then
      Throw New ArgumentException("filePath", "The specified file path is invalid.")
    End If

    ' Validate (and create if necessary) the destination directory.
    If Not FileUtilities.ValidateDirectoryPath(Path.GetDirectoryName(filePath), logger) Then
      Throw New ArgumentException("filePath", "The directory does not exist and could not be created.")
    End If

    Try

      ' Try to get the file stream.
      Using spriteFileStream As FileStream = FileUtilities.GetWriteFileStream(filePath, logger)

        ' Make sure we could open it.
        If spriteFileStream Is Nothing Then
          Return False
        End If

        ' Now serialize.
        SheetSerializer.Serialize(spriteFileStream, sheet)

        ' We're good.
        Return True

      End Using

    Catch ex As InvalidOperationException
      ' According to the XmlSerializer docs, the InnerException will hold
      ' the original exception.
      logger.Error("An error occured during serialization.", exception:=ex.InnerException)
    End Try

    Return False

  End Function

  ''' <summary>
  ''' Loads a sprite sheet from the specified file.
  ''' If this is unsuccessful, any error messages will be written to the
  ''' <paramref name="logger" />.
  ''' </summary>
  ''' <param name="filePath">The file path.</param>
  ''' <param name="logger">The logger.</param>
  ''' <returns>The instantiated sprite sheet, or <c>null</c> if it
  ''' could not be loaded.</returns>
  ''' <exception cref="System.ArgumentNullException">
  ''' <paramref name="filePath" /> is null or whitespace.
  ''' </exception>
  ''' <exception cref="System.ArgumentNullException">
  ''' <paramref name="logger" /> is null.
  ''' </exception>
  ''' <exception cref="System.ArgumentException">
  ''' The specified <paramref name="filePath" /> is an invalid file path.
  ''' </exception>
  ''' <exception cref="System.IO.FileNotFoundException">
  ''' The file specified by <paramref name="filePath" /> does not exist.
  ''' </exception>
  Public Shared Function LoadSpriteSheet(filePath As String, logger As ILogger) As SpriteSheet

    ' Validate our args
    If String.IsNullOrWhiteSpace(filePath) Then
      Throw New ArgumentNullException("filePath")
    End If

    If logger Is Nothing Then
      Throw New ArgumentNullException("logger")
    End If

    ' Validate the file path and that it exists
    If Not Validation.ValidateFilePath(filePath) Then
      Throw New ArgumentException("filePath", "The specified file path is invalid.")
    End If

    If Not File.Exists(filePath) Then
      Throw New FileNotFoundException("The file specified by """ & filePath & """ does not exist.")
    End If

    Try

      ' Try to get the file stream.
      Using spriteFileStream As FileStream = FileUtilities.GetReadFileStream(filePath, logger)

        ' Make sure we could open it.
        If spriteFileStream Is Nothing Then
          Return Nothing
        End If

        ' Deserialize the sprite sheet.
        Dim sheet As SpriteSheet = CType(SheetSerializer.Deserialize(spriteFileStream), SpriteSheet)

        ' Ensure the sprite collection isn't null.
        If sheet.Sprites Is Nothing Then
          sheet.Sprites = New List(Of Sprite)
        End If

        Return sheet

      End Using

    Catch ex As InvalidOperationException
      ' According to the XmlSerializer docs, the InnerException will hold
      ' the original exception.
      logger.Error("An error occured during deserialization.", exception:=ex.InnerException)
    End Try

    ' Something bad happened.
    Return Nothing

  End Function

  ''' <summary>
  ''' Relocates the <see cref="Sprite.ImagePath">image path</see>
  ''' and <see cref="Sprite.HoverImagePath">hover image path</see>
  ''' of each sprite to point to the specified directory.
  ''' </summary>
  ''' <param name="sheet">The sprite sheet.</param>
  ''' <param name="newImageDirectory">The new image directory.</param>
  ''' <exception cref="System.ArgumentNullException">
  ''' <paramref name="sheet" /> is <c>null</c>.
  ''' </exception>
  ''' <exception cref="System.ArgumentNullException">
  ''' <paramref name="newImageDirectory" /> is null or whitespace.
  ''' </exception>
  ''' <exception cref="System.ArgumentException">
  ''' The directory specified by <paramref name="newImageDirectory" /> is not valid.
  ''' </exception>
  Public Shared Sub RelocateImagePaths(sheet As SpriteSheet, newImageDirectory As String)

    ' Validate our args
    If sheet Is Nothing Then
      Throw New ArgumentNullException("sheet")
    End If

    If String.IsNullOrWhiteSpace(newImageDirectory) Then
      Throw New ArgumentNullException("newImageDirectory")
    End If

    ' Also validate that the new image directory is a valid path.
    If Not Validation.ValidateDirectoryPath(newImageDirectory) Then
      Throw New ArgumentException("newImageDirectory", "The directory specified by newImageDirectory is not valid.")
    End If

    ' Okay, now we update the sprites
    If sheet.Sprites IsNot Nothing AndAlso sheet.Sprites.Any() Then

      For Each sprite In sheet.Sprites

        ' Relocate the image path if it's defined
        If Not String.IsNullOrWhiteSpace(sprite.ImagePath) Then
          sprite.ImagePath = FileUtilities.RelocateFilePath(sprite.ImagePath, newImageDirectory)
        End If

        ' Relocate the hover image path if it's defined
        If Not String.IsNullOrWhiteSpace(sprite.HoverImagePath) Then
          sprite.HoverImagePath = FileUtilities.RelocateFilePath(sprite.HoverImagePath, newImageDirectory)
        End If

      Next

    End If

  End Sub

End Class
