Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports Tropical.Models

''' <summary>
''' A view model for interacting with a sprite sheet.
''' </summary>
Public Class SpriteSheetViewModel
  Inherits BaseNotifyPropertyChanged

  ''' <summary>
  ''' Initializes a new instance of the <see cref="SpriteSheetViewModel"/> class.
  ''' </summary>
  ''' <param name="spriteSheet">The sprite sheet.</param>
  ''' <param name="containingWindow">The containing window, to be used
  ''' for displaying any modal dialogs as necessary.</param>
  Public Sub New(spriteSheet As SpriteSheet,
                 containingWindow As Window)

    ' Set up our internals.
    Me._spriteSheet = spriteSheet
    Me._sprites = New ObservableCollection(Of Sprite)(spriteSheet.Sprites)
    Me._service = New SpriteSheetService(Me._sprites)
    Me._readOnlySprites = New ReadOnlyObservableCollection(Of Sprite)(Me._sprites)
    
    ' Wire up our commands
    Me._AddCommand = New AddCommand(
      Me.Service,
      Function() Me.CanAdd,
      Sub(sprite)
        Me.CurrentSprite = sprite
        Me.PropagateSpriteCollectionChanges()
      End Sub)

    Me._DeleteCommand = New DeleteCommand(
      Me.Service,
      Function() Me.CanDelete,
      Sub(sprite)
        Me.CurrentSprite = Nothing
        Me.PropagateSpriteCollectionChanges()
      End Sub)

    Me._MoveUpCommand = New MoveUpCommand(
      Me.Service,
      Function() Me.CanMoveUp,
      Sub(sprite)
        Me.PropagateSpriteCollectionChanges()
      End Sub)

    Me._MoveDownCommand = New MoveDownCommand(
      Me.Service,
      Function() Me.CanMoveDown,
      Sub(sprite)
        Me.PropagateSpriteCollectionChanges()
      End Sub)

    Me._BrowseImagePathCommand = New BrowseImagePathCommand(
      Me.Service,
      Function() Me.CanChangeImagePath,
      Nothing)

    Me._BrowseHoverImagePathCommand = New BrowseHoverImagePathCommand(
      Me.Service,
      Function() Me.CanChangeImagePath,
      Nothing)

    Me._ClearImagePathCommand = New ClearImagePathCommand(
      Me.Service,
      Function() Me.CanChangeImagePath,
      Nothing)

    Me._ClearHoverImagePathCommand = New ClearHoverImagePathCommand(
      Me.Service,
      Function() Me.CanChangeImagePath,
      Nothing)

  End Sub

  ''' <summary>
  ''' Propagates the changes made to the <see cref="Sprites" />
  ''' collection to the <see cref="SpriteSheet">sprite sheet's</see>
  ''' <see cref="SpriteSheet.Sprites">sprites list</see>.
  ''' </summary>
  Private Sub PropagateSpriteCollectionChanges()
    Me.SpriteSheet.Sprites = Me.Sprites.ToList()
  End Sub

  Private ReadOnly _spriteSheet As SpriteSheet

  ''' <summary>
  ''' Gets the sprite sheet represented by this view model.
  ''' Note that changes to this instance's <see cref="SpriteSheet.Sprites" />
  ''' collection will be ignored. Any necessary changes should be made
  ''' to <see cref="Sprites" />.
  ''' </summary>
  ''' <value>
  ''' The sprite sheet represented by this view model.
  ''' </value>
  Public ReadOnly Property SpriteSheet() As SpriteSheet
    Get
      Return Me._spriteSheet
    End Get
  End Property

  Private ReadOnly _sprites As ObservableCollection(Of Sprite)
  Private ReadOnly _readOnlySprites As ReadOnlyObservableCollection(Of Sprite)

  ''' <summary>
  ''' Gets the sprites in the sheet represented by this view model.
  ''' </summary>
  ''' <value>
  ''' The sprites in the sheet represented by this view model.
  ''' </value>
  Public ReadOnly Property Sprites As ReadOnlyObservableCollection(Of Sprite)
    Get
      Return Me._readOnlySprites
    End Get
  End Property

  Private ReadOnly _service As SpriteSheetService

  ''' <summary>
  ''' Gets the service used by this view model.
  ''' </summary>
  ''' <value>
  ''' The service used by this view model.
  ''' </value>
  Public ReadOnly Property Service() As SpriteSheetService
    Get
      Return Me._service
    End Get
  End Property

  Private ReadOnly _window As Window

  ''' <summary>
  ''' Gets the containing window, to be used for displaying
  ''' any modal dialogs as necessary.
  ''' </summary>
  ''' <value>
  ''' The containing window, to be used for displaying any
  ''' modal dialogs as necessary.
  ''' </value>
  Public ReadOnly Property ContainingWindow() As Window
    Get
      Return Me._window
    End Get
  End Property

  Private _currentSprite As Sprite

  ''' <summary>
  ''' Gets or sets the currently-selected sprite.
  ''' </summary>
  ''' <value>
  ''' The currently-selected sprite.
  ''' </value>
  Public Property CurrentSprite() As Sprite
    Get
      Return Me._currentSprite
    End Get
    Set(value As Sprite)

      If value IsNot Me._currentSprite Then

        ' We're skipping CanAdd here because it's
        ' always true.
        RaisePropertyChanging(Function() CurrentSprite)
        RaisePropertyChanging(Function() CanDelete)
        RaisePropertyChanging(Function() CanMoveUp)
        RaisePropertyChanging(Function() CanMoveDown)
        RaisePropertyChanging(Function() CanChangeImagePath)

        Me._currentSprite = value

        RaisePropertyChanged(Function() CurrentSprite)
        RaisePropertyChanged(Function() CanDelete)
        RaisePropertyChanged(Function() CanMoveUp)
        RaisePropertyChanged(Function() CanMoveDown)
        RaisePropertyChanged(Function() CanChangeImagePath)

      End If

    End Set
  End Property

  ''' <summary>
  ''' Handles when a property has changed.  Used to simplify the process
  ''' of invoking <see cref="ICommand.CanExecuteChanged" /> when corresponding
  ''' properties (such as <see cref="CanDelete" />) are changed.
  ''' </summary>
  ''' <param name="sender">The sender.</param>
  ''' <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
  Private Sub PropertyChangedHandler(sender As Object, e As PropertyChangedEventArgs) Handles Me.PropertyChanged

    Select Case e.PropertyName.ToLower()

      Case "canadd"
        AddCommand.RaiseCanExecuteChanged()

      Case "candelete"
        DeleteCommand.RaiseCanExecuteChanged()

      Case "canmoveup"
        MoveUpCommand.RaiseCanExecuteChanged()

      Case "canmovedown"
        MoveDownCommand.RaiseCanExecuteChanged()

      Case "canchangeimagepath"
        BrowseImagePathCommand.RaiseCanExecuteChanged()
        BrowseHoverImagePathCommand.RaiseCanExecuteChanged()
        ClearImagePathCommand.RaiseCanExecuteChanged()
        ClearHoverImagePathCommand.RaiseCanExecuteChanged()

    End Select

  End Sub

#Region "Sprite Sheet Loading"

  ''' <summary>
  ''' Occurs when a new sprite sheet has been loaded,
  ''' which notifies listeners that a new view model
  ''' should be created from that sheet.
  ''' </summary>
  Public Event NewSpriteSheetLoaded(newSheet As SpriteSheet)

#End Region

#Region "Command Implementation"

  ''' <summary>
  ''' Gets a value indicating whether sprites can be
  ''' added to the sheet.
  ''' </summary>
  ''' <value>
  ''' <c>true</c> if sprites can be added
  ''' to the sheet; otherwise, <c>false</c>.
  ''' </value>
  Public ReadOnly Property CanAdd() As Boolean
    Get
      ' Right now, we can always add sprites to the sheet
      Return True
    End Get
  End Property

  ''' <summary>
  ''' Gets a value indicating whether the current sprite can be
  ''' removed from the sheet.
  ''' </summary>
  ''' <value>
  ''' <c>true</c> if the current sprite can be removed
  ''' from the sheet; otherwise, <c>false</c>.
  ''' </value>
  Public ReadOnly Property CanDelete() As Boolean
    Get
      Return Me.CurrentSprite IsNot Nothing
    End Get
  End Property

  ''' <summary>
  ''' Gets a value indicating whether the current sprite can be
  ''' moved up in the sheet.
  ''' </summary>
  ''' <value>
  ''' <c>true</c> if the current sprite can be moved up
  ''' in the sheet; otherwise, <c>false</c>.
  ''' </value>
  Public ReadOnly Property CanMoveUp() As Boolean
    Get
      Return Me.CurrentSprite IsNot Nothing
    End Get
  End Property

  ''' <summary>
  ''' Gets a value indicating whether the current sprite can be
  ''' moved down in the sheet.
  ''' </summary>
  ''' <value>
  ''' <c>true</c> if the current sprite can be moved down
  ''' in the sheet; otherwise, <c>false</c>.
  ''' </value>
  Public ReadOnly Property CanMoveDown() As Boolean
    Get
      Return Me.CurrentSprite IsNot Nothing
    End Get
  End Property

  ''' <summary>
  ''' Gets a value indicating whether the current sprite can have
  ''' its image paths changed.
  ''' </summary>
  ''' <value>
  ''' <c>true</c> if the current sprite can have its
  ''' image paths changed; otherwise, <c>false</c>.
  ''' </value>
  Public ReadOnly Property CanChangeImagePath() As Boolean
    Get
      Return Me.CurrentSprite IsNot Nothing
    End Get
  End Property

  Private ReadOnly _addCommand As AddCommand

  ''' <summary>
  ''' Gets the command used to add a new sprite.
  ''' </summary>
  ''' <value>
  ''' The command used to add a new sprite.
  ''' </value>
  Public ReadOnly Property AddCommand As AddCommand
    Get
      Return Me._addCommand
    End Get
  End Property

  Private ReadOnly _deleteCommand As DeleteCommand

  ''' <summary>
  ''' Gets the command used to delete a sprite.
  ''' </summary>
  ''' <value>
  ''' The command used to delete a sprite.
  ''' </value>
  Public ReadOnly Property DeleteCommand As DeleteCommand
    Get
      Return Me._deleteCommand
    End Get
  End Property

  Private ReadOnly _moveUpCommand As MoveUpCommand

  ''' <summary>
  ''' Gets the command used to move up a sprite in the list.
  ''' </summary>
  ''' <value>
  ''' The command used to move up a sprite in the list.
  ''' </value>
  Public ReadOnly Property MoveUpCommand As MoveUpCommand
    Get
      Return Me._moveUpCommand
    End Get
  End Property

  Private ReadOnly _moveDownCommand As MoveDownCommand

  ''' <summary>
  ''' Gets the command used to move down a sprite in the list.
  ''' </summary>
  ''' <value>
  ''' The command used to move down a sprite in the list.
  ''' </value>
  Public ReadOnly Property MoveDownCommand As MoveDownCommand
    Get
      Return Me._moveDownCommand
    End Get
  End Property

  Private ReadOnly _browseImagePathCommand As BrowseImagePathCommand

  ''' <summary>
  ''' Gets the command used to browse for a file to use
  ''' as a sprite's <see cref="Sprite.ImagePath" />.
  ''' </summary>
  ''' <value>
  ''' The command used to browse for a sprite's <see cref="Sprite.ImagePath" />.
  ''' </value>
  Public ReadOnly Property BrowseImagePathCommand As BrowseImagePathCommand
    Get
      Return Me._browseImagePathCommand
    End Get
  End Property

  Private ReadOnly _browseHoverImagePathCommand As BrowseHoverImagePathCommand

  ''' <summary>
  ''' Gets the command used to browse for a file to use
  ''' as a sprite's <see cref="Sprite.HoverImagePath" />.
  ''' </summary>
  ''' <value>
  ''' The command used to browse for a sprite's <see cref="Sprite.HoverImagePath" />.
  ''' </value>
  Public ReadOnly Property BrowseHoverImagePathCommand As BrowseHoverImagePathCommand
    Get
      Return Me._browseHoverImagePathCommand
    End Get
  End Property

  Private ReadOnly _clearImagePathCommand As ClearImagePathCommand

  ''' <summary>
  ''' Gets the command used to browse for a file to clear out
  ''' a sprite's <see cref="Sprite.ImagePath" />.
  ''' </summary>
  ''' <value>
  ''' The command used to clear out a sprite's <see cref="Sprite.ImagePath" />.
  ''' </value>
  Public ReadOnly Property ClearImagePathCommand As ClearImagePathCommand
    Get
      Return Me._clearImagePathCommand
    End Get
  End Property

  Private ReadOnly _clearHoverImagePathCommand As ClearHoverImagePathCommand

  ''' <summary>
  ''' Gets the command used to browse for a file to clear out
  ''' a sprite's <see cref="Sprite.HoverImagePath" />.
  ''' </summary>
  ''' <value>
  ''' The command used to clear out a sprite's <see cref="Sprite.HoverImagePath" />.
  ''' </value>
  Public ReadOnly Property ClearHoverImagePathCommand As ClearHoverImagePathCommand
    Get
      Return Me._clearHoverImagePathCommand
    End Get
  End Property

#End Region

End Class
