
-- MusiC

package = newpackage()
package.name="MusiC"
package.path=project.path

-- Input
package.language="c#"
package.files={
	matchrecursive(base_src_dir.."/Music/*.cs"),
	matchrecursive(base_src_dir.."/*.xml")
}
package.links={
	"System",
	"System.Xml",
	"log4net"
}
package.libpaths={
	base_deps_dir.."/DotNet"
}
-- Code Generation
package.defines={"TRACE"}
package.buildflags={"unsafe"}

-- Output
package.kind="dll"
package.target="MusiC"
package.targetprefix=""
package.targetextension="dll"
package.objdir=base_bin_dir.."/obj"

-- Debug:MusiC
package.config["Debug"].defines={"DEBUG"}
