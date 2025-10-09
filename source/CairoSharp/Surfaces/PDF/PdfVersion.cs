// (c) gfoidl, all rights reserved

using static Cairo.Surfaces.PDF.PdfSurfaceNative;

namespace Cairo.Surfaces.PDF;

/// <summary>
/// <see cref="PdfVersion"/> is used to describe the version number of the PDF
/// specification that a generated PDF file will conform to.
/// </summary>
public enum PdfVersion
{
    /// <summary>
    /// The version 1.4 of the PDF specification. (Since 1.10)
    /// </summary>
    Version1_4,

    /// <summary>
    /// The version 1.5 of the PDF specification. (Since 1.10)
    /// </summary>
    Version1_5,

    /// <summary>
    /// The version 1.6 of the PDF specification. (Since 1.18)
    /// </summary>
    Version1_6,

    /// <summary>
    /// The version 1.7 of the PDF specification. (Since 1.18)
    /// </summary>
    Version1_7
}

public static unsafe class PdfVersionExtensions
{
    extension(PdfVersion version)
    {
        /// <summary>
        /// Get the string representation of the given version id. This method will return <c>null</c> if
        /// version isn't valid. See <see cref="GetSupportedVersions"/> for a way to get the list of
        /// valid version ids.
        /// </summary>
        /// <returns>the string associated to given version.</returns>
        public string? GetString()
        {
            sbyte* tmp = cairo_pdf_version_to_string(version);
            return tmp is not null ? new string(tmp) : null;
        }
    }

    extension(PdfSurface)
    {
        /// <summary>
        /// Used to retrieve the list of supported versions. See <see cref="PdfSurface.RestrictToVersion(PdfVersion)"/>.
        /// </summary>
        /// <returns>supported version list</returns>
        public static ReadOnlySpan<PdfVersion> GetSupportedVersions()
        {
            cairo_pdf_get_versions(out PdfVersion* versions, out int length);
            return new ReadOnlySpan<PdfVersion>(versions, length);
        }
    }
}
