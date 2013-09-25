Imports System.IO

''' <summary>
''' Contains utilities for working with files, directories, and paths.
''' </summary>
Public Class FileUtilities

  ''' <summary>
  ''' Prevents a default instance of the <see cref="FileUtilities"/> class from being created.
  ''' </summary>
  Private Sub New()
  End Sub

  ''' <summary>
  ''' Validates the provided directory path. If the directory does not exist,
  ''' this method will attempt to create it.
  ''' If this is unsuccessful, this method will return <c>false</c> and log
  ''' any errors in the provided <paramref name="logger" />.
  ''' </summary>
  ''' <param name="directoryPath">The directory path.</param>
  ''' <param name="logger">The logger.</param>
  ''' <returns><c>true</c> if the provided directory path is valid;
  ''' <c>false</c> otherwise.</returns>
  ''' <exception cref="System.ArgumentNullException">
  ''' <paramref name="filePath" /> is null or whitespace.
  ''' </exception>
  ''' <exception cref="System.ArgumentNullException">
  ''' <paramref name="logger" /> is null.
  ''' </exception>
  Public Shared Function ValidateDirectoryPath(directoryPath As String,
                                               logger As Models.ILogger) As Boolean

    If String.IsNullOrWhiteSpace(directoryPath) Then
      Throw New ArgumentNullException("directoryPath")
    End If

    If logger Is Nothing Then
      Throw New ArgumentNullException("logger")
    End If

    ' First validate the path
    If Not Validation.ValidateDirectoryPath(directoryPath) Then
      Return False
    End If

    Try

      ' Try to create it if it doesn't exist.
      If Not Directory.Exists(directoryPath) Then
        Directory.CreateDirectory(directoryPath)
      End If

      ' We're good, as it either exists or we created it without error.
      Return True

    Catch ex As DirectoryNotFoundException
      logger.Error("The directory path """ & directoryPath & """ is invalid.", exception:=ex)
    Catch ex As PathTooLongException
      logger.Error("The directory path """ & directoryPath & """ is too long.", exception:=ex)
    Catch ex As UnauthorizedAccessException
      logger.Error("The directory """ & directoryPath & """ could not be created. You may not have permission to create it.", exception:=ex)
    Catch ex As NotSupportedException
      logger.Error("The directory """ & directoryPath & """ could not be created.", exception:=ex)
    Catch ex As IOException
      logger.Error("The directory """ & directoryPath & """ could not be created. It may be a path to a file.", exception:=ex)
    End Try

    ' We hit an error up above, so this isn't valid.
    Return False

  End Function

  ''' <summary>
  ''' Gets an exclusive file stream for the file specified by <paramref name="filePath" />.
  ''' This will overwrite any contents of the file if it already exists.
  ''' If this is unsuccessful, this method will return <c>null</c> and log
  ''' any errors in the provided <paramref name="logger" />.
  ''' </summary>
  ''' <param name="filePath">The path to the file. It is assumed
  ''' that this file path has already been validated.</param>
  ''' <param name="logger">The logger.</param>
  ''' <returns>The requested stream for the file, or <c>null</c>
  ''' if the stream could not be retrieved.</returns>
  ''' <exception cref="System.ArgumentNullException">
  ''' <paramref name="filePath" /> is null or whitespace.
  ''' </exception>
  ''' <exception cref="System.ArgumentNullException">
  ''' <paramref name="logger" /> is null.
  ''' </exception>
  Public Shared Function GetWriteFileStream(filePath As String,
                                            logger As Models.ILogger) As FileStream

    ' Validate args.
    If String.IsNullOrWhiteSpace(filePath) Then
      Throw New ArgumentNullException("filePath")
    End If

    If logger Is Nothing Then
      Throw New ArgumentNullException("logger")
    End If

    ' Get the file name, which we'll use for error messages.
    Dim fileName As String = Path.GetFileName(filePath)

    Try
      ' This will get exclusivity on the file handle,
      ' which is what we want.
      Return File.Create(filePath)
    Catch ex As DirectoryNotFoundException
      logger.Error("The file path """ & filePath & """ refers to a directory that does not exist.", exception:=ex)
    Catch ex As PathTooLongException
      logger.Error("The file path """ & filePath & """ is too long.", exception:=ex)
    Catch ex As UnauthorizedAccessException
      logger.Error("The file """ & fileName & """ could not be created. It may be opened by another program or marked as read-only.", exception:=ex)
    Catch ex As IOException
      logger.Error("The file """ & fileName & """ could not be created.", exception:=ex)
    End Try

    Return Nothing

  End Function

  ''' <summary>
  ''' Gets a file stream (for opening) for the file specified by <paramref name="filePath" />.
  ''' If this is unsuccessful, this method will return <c>null</c> and log
  ''' any errors in the provided <paramref name="logger" />.
  ''' </summary>
  ''' <param name="filePath">The path to the file. It is assumed
  ''' that this file path has already been validated.</param>
  ''' <param name="logger">The logger.</param>
  ''' <returns>The requested stream for the file, or <c>null</c>
  ''' if the stream could not be retrieved.</returns>
  ''' <exception cref="System.ArgumentNullException">
  ''' <paramref name="filePath" /> is null or whitespace.
  ''' </exception>
  ''' <exception cref="System.ArgumentNullException">
  ''' <paramref name="logger" /> is null.
  ''' </exception>
  Public Shared Function GetReadFileStream(filePath As String,
                                           logger As Models.ILogger) As FileStream

    ' Validate args.
    If String.IsNullOrWhiteSpace(filePath) Then
      Throw New ArgumentNullException("filePath")
    End If

    If logger Is Nothing Then
      Throw New ArgumentNullException("logger")
    End If

    ' Get the file name, which we'll use for error messages.
    Dim fileName As String = Path.GetFileName(filePath)

    Try
      ' Get the file stream for just opening
      Return File.OpenRead(filePath)
    Catch ex As FileNotFoundException
      logger.Error("The file path """ & filePath & """ refers to a file that does not exist.", exception:=ex)
    Catch ex As DirectoryNotFoundException
      logger.Error("The file path """ & filePath & """ refers to a directory that does not exist.", exception:=ex)
    Catch ex As PathTooLongException
      logger.Error("The file path """ & filePath & """ is too long.", exception:=ex)
    Catch ex As UnauthorizedAccessException
      logger.Error("The file """ & fileName & """ could not be opened. You may not have access to it.", exception:=ex)
    Catch ex As NotSupportedException
      logger.Error("The file path """ & filePath & """ is in an invalid format.", exception:=ex)
    End Try

    Return Nothing

  End Function

  ''' <summary>
  ''' Relocates a file path such that it will refer to a file
  ''' in the new directory.
  ''' </summary>
  ''' <param name="filePath">The file path to relocate. It is assumed
  ''' that this file path has already been validated.</param>
  ''' <param name="newDirectoryPath">The new directory path. It is assumed
  ''' that this directory path has already been validated.</param>
  ''' <returns>The relocated file path.</returns>
  ''' <exception cref="System.ArgumentNullException">
  ''' <paramref name="filePath" /> is null or whitespace.
  ''' </exception>
  ''' <exception cref="System.ArgumentNullException">
  ''' <paramref name="newDirectory" /> is null or whitespace.
  ''' </exception>
  Public Shared Function RelocateFilePath(filePath As String,
                                          newDirectoryPath As String) As String

    If String.IsNullOrWhiteSpace(filePath) Then
      Throw New ArgumentNullException("filePath")
    End If

    If String.IsNullOrWhiteSpace(newDirectoryPath) Then
      Throw New ArgumentNullException("directory")
    End If

    Return Path.Combine(newDirectoryPath, Path.GetFileName(filePath))

  End Function

End Class
