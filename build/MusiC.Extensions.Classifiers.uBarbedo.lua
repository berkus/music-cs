-- MusiC.Extensions.Classifiers.uBarbedo

package = newpackage()
package.name="MusiC.Extensions.Classifiers.uBarbedo"

-- Input
package.language="c++"
package.files={
	matchrecursive("../src/Extensions/Classifiers/Barbedo/*.cpp")
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
package.target="MusiC.Extensions.Classifiers.uBarbedo"
package.targetprefix=""
package.targetextension="dll"
package.objdir="../bin/obj"

-- Debug:MusiC
package.config["Debug"].defines={"DEBUG"}
