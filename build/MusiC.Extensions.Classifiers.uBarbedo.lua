-- MusiC.Extensions.Classifiers.uBarbedo

package = newpackage()
package.name="MusiC.Extensions.Classifiers.uBarbedo"
package.path=project.path

-- Input
package.language="c++"
package.files={
	matchrecursive(base_src_dir.."/Extensions/Classifiers/Barbedo/*.cpp")
}
package.links={
	"gsl",
	"gslcblas"
}
package.libpaths={
	base_deps_dir.."/"..OS.."/lib"
}

package.includepaths={
	base_deps_dir.."/include"
}

-- Code Generation
--package.defines={"TRACE"}
package.buildflags={"dylib"}

-- Output
package.kind="dll"
package.target="MusiC.Extensions.Classifiers.uBarbedo"
package.targetprefix=""
package.targetextension="dll"
package.bindir=base_bin_dir.."/Extensions"
package.objdir="obj"

-- Debug:MusiC
package.config["Debug"].defines={"DEBUG"}
