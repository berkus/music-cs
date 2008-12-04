
-- MusiC

package = newpackage()
package.name="MusiC"

-- Input
package.language="c#"
package.files={
	matchrecursive("../src/Music/*.cs")
}
package.links={
	"System",
	"System.Xml",
	"log4net"
}
package.libpaths={
	"../deps/DotNet"
}
-- Code Generation
package.defines={"TRACE"}
package.buildflags={"unsafe"}

-- Output
package.kind="dll"
package.target="MusiC"
package.targetprefix=""
package.targetextension="dll"
package.objdir="../bin/obj"

-- Debug:MusiC
package.config["Debug"].defines={"DEBUG"}
