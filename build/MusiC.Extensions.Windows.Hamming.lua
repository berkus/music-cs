-- MusiC.Extensions.Windows.Hamming

package = newpackage()
package.name="MusiC.Extensions.Windows.Hamming"

-- Input
package.language="c#"
package.files={
	matchrecursive("../src/Extensions/Windows/Hamming/*.cs")
}
package.links={
	"System",
	"MusiC"
}
-- Code Generation
package.defines={"TRACE"}
package.buildflags={"unsafe"}

-- Output
package.kind="dll"
package.target="MusiC.Extensions.Windows.Hamming"
package.targetprefix=""
package.targetextension="dll"
package.objdir="../bin/obj"

-- Debug:MusiC
package.config["Debug"].defines={"DEBUG"}
