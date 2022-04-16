using System.Xml;
using System.Xml.Xsl;


var len = Environment.GetCommandLineArgs().Length;
if( len != 4 )
{
    Console.WriteLine( "Usage: xslttool.exe [input.xml] [stylesheet.xsl] [output.html]" );
    return;
}

// args
string filename = Environment.GetCommandLineArgs()[1];
string stylesheet = Environment.GetCommandLineArgs()[2];
string htmlfile = Environment.GetCommandLineArgs()[3];
Console.WriteLine( $"filename:   [{filename}]" );
Console.WriteLine( $"stylesheet: [{stylesheet}]" );
Console.WriteLine( $"htmlfile:   [{htmlfile}]" );

// Create a resolver with default credentials.
XmlUrlResolver resolver = new XmlUrlResolver();
resolver.Credentials = System.Net.CredentialCache.DefaultCredentials;

// Create the XslTransform object.
XslTransform xslt = new XslTransform();

// Load the stylesheet.
xslt.Load( stylesheet, resolver );

// Transform the file.
xslt.Transform( filename, htmlfile, resolver );
