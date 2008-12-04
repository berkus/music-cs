-- MusiC.Extensions.Handlers.uDBHandler

package = newpackage()
package.name="MusiC.Extensions.Handlers.uDBHandler"

-- Input
package.language="c++"
package.files={
	matchrecursive("../src/Extensions/Handlers/DBHandler/*.cpp")
}
package.links={
	"gsl",
	"gslcblas"
}
package.libpaths={
	"../deps/"..OS.."/lib"
}
package.includepaths={
    "../deps/include"
}

-- Code Generation
--package.defines={"TRACE"}
package.buildflags={"dylib"}

-- Output
package.kind="dll"
package.target="MusiC.Extensions.Handlers.uDBHandler"
package.targetprefix=""
package.targetextension="dll"
package.objdir="../bin/obj"

-- Debug:MusiC
package.config["Debug"].defines={"DEBUG"}
