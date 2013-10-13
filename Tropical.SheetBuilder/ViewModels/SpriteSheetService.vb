Imports System.Collections.ObjectModel
Imports Tropical.Models
Imports Tropical.Controllers

''' <summary>
''' Contains methods for interacting with a sprite sheet
''' and its sprites collection.
''' </summary>
Public Class SpriteSheetService

  Private ReadOnly _sprites As ObservableCollection(Of Sprite)

  ''' <summary>
  ''' Initializes a new instance of the <see cref="SpriteSheetService"/> class.
  ''' </summary>
  ''' <param name="sprites">The sprites collection
  ''' to perform operations on.</param>
  Public Sub New(sprites As ObservableCollection(Of Sprite))
    _sprites = sprites
  End Sub

  ''' <summary>
  ''' Adds a sprite to the list.
  ''' </summary>
  ''' <param name="insertAtSprite">If specified, the position
  ''' of the sprite to insert the new sprite at. If not specified,
  ''' or not in the list, the new sprite will be added to
  ''' the end of the list.</param>
  ''' <returns>The created sprite.</returns>
  Public Function AddSprite(insertAtSprite As Sprite) As Sprite

    Dim createdSprite As New Sprite()
    Dim insertIndex As Int32 = -1

    If insertAtSprite IsNot Nothing Then
      insertIndex = _sprites.IndexOf(insertAtSprite)
    End If

    ' If we didn't find the sprite (or didn't have one), insert it at the end
    If insertIndex >= 0 Then
      _sprites.Insert(insertIndex, createdSprite)
    Else
      _sprites.Add(createdSprite)
    End If

    Return createdSprite

  End Function

  ''' <summary>
  ''' Deletes a sprite from the list.
  ''' </summary>
  ''' <param name="sprite">The sprite to delete.</param>
  ''' <returns><c>true</c> if the sprite was deleted successfully;
  ''' otherwise, <c>false</c>.</returns>
  Public Function DeleteSprite(sprite As Sprite) As Boolean
    Return _sprites.Remove(sprite)
  End Function

  ''' <summary>
  ''' Moves the sprite in the list.
  ''' </summary>
  ''' <param name="sprite">The sprite to move.</param>
  ''' <param name="moveUp">If set to <c>true</c>, indicates that
  ''' the sprite should be moved up. If set to <c>false</c>, indicates
  ''' that the sprite should be moved down.</param>
  ''' <returns><c>true</c> if the sprite was moved successfully;
  ''' otherwise, <c>false</c>.</returns>
  Public Function MoveSprite(sprite As Sprite, moveUp As Boolean) As Boolean

    Dim oldSpriteIndex As Int32
    Dim newSpriteIndex As Int32

    If sprite IsNot Nothing Then

      ' Find the sprite
      oldSpriteIndex = _sprites.IndexOf(sprite)

      If oldSpriteIndex <> -1 Then

        ' Figure out the new sprite index
        If moveUp Then
          newSpriteIndex = oldSpriteIndex - 1
        Else
          newSpriteIndex = oldSpriteIndex + 1
        End If

        ' Keep the new index within the bounds of the collection
        newSpriteIndex = Math.Min(_sprites.Count - 1, Math.Max(0, newSpriteIndex))

        ' If they differ, move and return true.
        If newSpriteIndex <> oldSpriteIndex Then
          _sprites.Move(oldSpriteIndex, newSpriteIndex)
          Return True
        End If

      End If

    End If

    ' Something didn't work out.
    Return False

  End Function

  ''' <summary>
  ''' Loads the sprite sheet file at the specified file path
  ''' and returns it.
  ''' </summary>
  ''' <param name="spriteSheetPath">The path to the sprite sheet file.</param>
  ''' <param name="logger">The logger.</param>
  ''' <returns>The loaded sprite sheet, or <c>null</c>
  ''' if the sprite sheet could not be loaded.</returns>
  Public Shared Function LoadSpriteSheet(spriteSheetPath As String,
                                         logger As ILogger) As SpriteSheet

    Try

      Dim sheet As SpriteSheet = SpriteSheetFileUtilities.LoadSpriteSheet(spriteSheetPath, logger)
      Return sheet

    Catch ex As Exception
      ' Uh-oh. At this point we're going to assume the
      ' message is something we've specified, so the message
      ' should be OK for user consumption.
      logger.Error(ex.Message, ex)
      Return Nothing
    End Try

  End Function

  ''' <summary>
  ''' Saves the sprite sheet to the specified path.
  ''' </summary>
  ''' <param name="savePath">The path to the sprite sheet.</param>
  ''' <param name="spriteSheet">The sprite sheet to save.</param>
  ''' <param name="logger">The logger.</param>
  ''' <returns><c>true</c> if the sprite sheet was successfully saved,
  ''' <c>false</c> otherwise.</returns>
  Public Shared Function SaveSpriteSheet(savePath As String,
                                         spriteSheet As SpriteSheet,
                                         logger As ILogger) As Boolean

    Try

      SpriteSheetFileUtilities.SaveSpriteSheet(savePath, spriteSheet, logger)
      Return True

    Catch ex As Exception
      ' Uh-oh. At this point we're going to assume the
      ' message is something we've specified, so the message
      ' should be OK for user consumption.
      logger.Error(ex.Message, ex)
      Return False
    End Try

  End Function

  ''' <summary>
  ''' Saves the generated sprite sheet (image, CSS file, and sample
  ''' HTML) to the specified path.
  ''' </summary>
  ''' <param name="savePath">The path to the directtory where
  ''' the sprite sheet files will be saved.</param>
  ''' <param name="spriteSheet">The sprite sheet to save.</param>
  ''' <param name="logger">The logger.</param>
  ''' <returns><c>true</c> if the sprite sheet was successfully
  ''' created, <c>false</c> otherwise.</returns>
  Public Shared Function SaveSpriteSheetContent(savePath As String,
                                                spriteSheet As SpriteSheet,
                                                logger As ILogger) As Boolean

    Try

      Dim generator As New SpriteSheetGenerator(spriteSheet)
      generator.Logger = logger

      Return generator.Generate(savePath)

    Catch ex As Exception
      ' Uh-oh. At this point we're going to assume the
      ' message is something we've specified, so the message
      ' should be OK for user consumption.      
      logger.Error(ex.Message, ex)
      Return False
    End Try

  End Function

End Class
