-- MusiC.Extensions.Classifiers.Barbedo

package = newpackage()
package.name="MusiC.Extensions.Classifiers.Barbedo"

-- Input
package.language="c#"
package.files={
	matchrecursive("../src/Extensions/Classifiers/Barbedo/*.cs")
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
package.target="MusiC.Extensions.Classifiers.Barbedo"
package.targetprefix=""
package.targetextension="dll"
package.objdir="../bin/obj"

-- Debug:MusiC
package.config["Debug"].defines={"DEBUG"}