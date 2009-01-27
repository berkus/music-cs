using System;
using System.IO;
using System.Collections.Specialized;

class app
{
    public static void Main()
    {
        if (File.Exists("printSrc.tex"))
            File.Delete("printSrc.tex");

        TextWriter f = new StreamWriter(new FileStream("printSrc.tex", FileMode.CreateNew));
        
        f.WriteLine("\\documentclass[a4paper]{article}");
        f.WriteLine("\\usepackage[T1]{fontenc}");
        f.WriteLine("\\usepackage[latin9]{inputenc}");
        f.WriteLine("\\usepackage[brazil]{babel}");
        f.WriteLine("\\usepackage{listings}");
        f.WriteLine("\\begin{document}");
		f.WriteLine("\\tableofcontents");
		
		f.WriteLine("\\lstset{tabsize=2,basicstyle=\\small,breaklines=true,showstringspaces=false}");
		f.WriteLine("\\lstset{numbers=left, numberstyle=\\tiny, stepnumber=2, numbersep=5pt}");
        
		f.WriteLine("\\lstset{language=[Sharp]C}");
		f.WriteLine("\\section{C Sharp}");
        exportToLatex(".", ".cs", f);

		f.WriteLine("\\lstset{language=[ANSI]C}");
		f.WriteLine("\\section{C++ Headers}");
		exportToLatex(".", ".h", f);		
		f.WriteLine("\\lstset{language=[ANSI]C++}");
		f.WriteLine("\\section{C++ Source}");
		exportToLatex(".", ".cpp", f);
		
		f.WriteLine("\\lstset{language=XML}");	
		f.WriteLine("\\section{XML Configuration Script}");
		exportToLatex(".", ".xml", f);
		
		f.WriteLine("\\lstset{language=Lua}");
		f.WriteLine("\\section{Premake Build Scripts}");
		exportToLatex("../build", ".lua", f);

        f.WriteLine("\\end{document}");
        f.Close();
    }

    public static void exportToLatex(string dir, string ext, TextWriter f)
    {
        string[] dirList = Directory.GetDirectories(dir);

        if (dirList.GetLength(0) > 0)
        {
            foreach (string d in dirList)
            {
                exportToLatex(d, ext, f);
            }
        }
        
        foreach (string file in Directory.GetFiles(dir))
        {
            if (Path.GetExtension(file) == ext && Path.GetFileName(file) != "AssemblyInfo.cs")
            {
                string relPath = RelativePathTo(Directory.GetCurrentDirectory(), file);
                relPath = relPath.Replace('\\', '/');
                f.WriteLine("\\subsection{" + relPath + "}");
                f.WriteLine("\\lstinputlisting{" + relPath + "}");
            }
        }
    }

    public static string RelativePathTo(string fromDirectory, string toPath)
    {

        if (fromDirectory == null)
            throw new ArgumentNullException("fromDirectory");

        if (toPath == null)
            throw new ArgumentNullException("toPath");

        bool isRooted = Path.IsPathRooted(fromDirectory)
            && Path.IsPathRooted(toPath);

        if (isRooted)
        {
            bool isDifferentRoot = string.Compare(
                Path.GetPathRoot(fromDirectory),
                Path.GetPathRoot(toPath), true) != 0;
            
            if (isDifferentRoot)
                return toPath;
        }

        StringCollection relativePath = new StringCollection();

        string[] fromDirectories = fromDirectory.Split(
            Path.DirectorySeparatorChar);

        string[] toDirectories = toPath.Split(
            Path.DirectorySeparatorChar);

        int length = Math.Min(
            fromDirectories.Length,
            toDirectories.Length);

        int lastCommonRoot = -1;

        // find common root
        for (int x = 0; x < length; x++)
        {
            if (string.Compare(fromDirectories[x],
                toDirectories[x], true) != 0)
                break;

            lastCommonRoot = x;
        }

        if (lastCommonRoot == -1)
            return toPath; 

        // add relative folders in from path
        for (int x = lastCommonRoot + 1; x < fromDirectories.Length; x++)
            if (fromDirectories[x].Length > 0)
                relativePath.Add("..");

        // add to folders to path
        for (int x = lastCommonRoot + 1; x < toDirectories.Length; x++)
            relativePath.Add(toDirectories[x]);

        // create relative path
        string[] relativeParts = new string[relativePath.Count];

        relativePath.CopyTo(relativeParts, 0);

        string newPath = string.Join(
            Path.DirectorySeparatorChar.ToString(),
            relativeParts);

        return newPath;

    }
}
