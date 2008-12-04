
DEPS_INCLUDE_FLAG=-Ideps/include
DEPS_LIB_FLAG=-Ldeps/mac/lib
DEPS_BIN_DIR=deps/mac/bin
MNG_DEPS_BIN_DIR=deps/DotNet

#TRACE_FLAG=-debug -define:TRACE
TRACE_FLAG=-define:TRACE

################################################################################
#################################### TARGETS ###################################
################################################################################

.PHONY= all dirStructure musiC classifiers windows features handlers applications dependency clean extensions

all: dirStructure dependency musiC extensions applications
extensions: classifiers windows features handlers configs

clean:
	rm -rf bin

#####################
### DIR STRUCTURE ###
#####################

bin:
	mkdir bin

bin/Extensions:
	mkdir bin/Extensions

dirStructure:bin bin/Extensions

###################
### CLASSIFIERS ###
###################

MBARBEDO=bin/Extensions/mBarbedo.dll
MBARBEDO_SRC=$(shell find src/Extensions/Classifiers/Barbedo -name "*.cs")

$(MBARBEDO): $(MBARBEDO_SRC)
	gmcs -out:$(MBARBEDO) -target:library -unsafe+ $(TRACE_FLAG) $(MBARBEDO_SRC) -r:$(MUSIC_BIN)

UBARBEDO=bin/Extensions/uBarbedo.dll
UBARBEDO_SRC=$(shell find src/Extensions/Classifiers/Barbedo -name "*.cpp")

$(UBARBEDO): $(UBARBEDO_SRC)
	g++ -dynamiclib -Wall -o $(UBARBEDO) $(UBARBEDO_SRC) -lgsl -lgslcblas $(DEPS_INCLUDE_FLAG) $(DEPS_LIB_FLAG)
	
.PHONY=barbedo
barbedo: $(MBARBEDO) $(UBARBEDO)

classifiers: barbedo

###############
### WINDOWS ###
###############

HAMMING=bin/Extensions/Hamming.dll
HAMMING_SRC=$(shell find src/Extensions/Windows/Hamming -name "*.cs")

$(HAMMING): $(HAMMING_SRC)
	gmcs -out:$(HAMMING) -target:library -unsafe+ $(TRACE_FLAG) $(HAMMING_SRC) -r:$(MUSIC_BIN)
	
windows:$(HAMMING)

################
### FEATURES ###
################

SPECROLLOFF=bin/Extensions/SpecRollOff.dll
SPECROLLOFF_SRC=$(shell find src/Extensions/Features/SpecRollOff -name "*.cs")

$(SPECROLLOFF): $(SPECROLLOFF_SRC)
	gmcs -out:$(SPECROLLOFF) -target:library -unsafe+ $(TRACE_FLAG) $(SPECROLLOFF_SRC) -r:$(MUSIC_BIN)

features: $(SPECROLLOFF)

################
### HANDLERS ###
################

MDBHANDLER=bin/Extensions/mDBHandler.dll
MDBHANDLER_SRC=$(shell find src/Extensions/Handlers/DBHandler -name "*.cs")

$(MDBHANDLER): $(MDBHANDLER_SRC)
	gmcs -unsafe+ -out:$(MDBHANDLER) -target:library $(TRACE_FLAG) $(MDBHANDLER_SRC) -r:$(MUSIC_BIN)

UDBHANDLER=bin/Extensions/uDBHandler.dll
UDBHANDLER_SRC=$(shell find src/Extensions/Handlers/DBHandler -name "*.cpp")

$(UDBHANDLER): $(UDBHANDLER_SRC)
	g++ -dynamiclib -Wall -o $(UDBHANDLER) $(UDBHANDLER_SRC) -lgsl -lgslcblas $(DEPS_INCLUDE_FLAG) $(DEPS_LIB_FLAG)

WAVHANDLER=bin/Extensions/WAVHandler.dll
WAVHANDLER_SRC=$(shell find src/Extensions/Handlers/WAVHandler -name "*.cs")

$(WAVHANDLER): $(WAVHANDLER_SRC)
	gmcs -unsafe+ -out:$(WAVHANDLER) -target:library $(TRACE_FLAG) $(WAVHANDLER_SRC) -r:$(MUSIC_BIN)
	
handlers: $(MDBHANDLER) $(UDBHANDLER) $(WAVHANDLER)

###############
### CONFIGS ###
###############

XMLCONFIG=bin/Extensions/XMLConfig.dll
XMLCONFIG_SRC=$(shell find src/Extensions/Configs/XMLConfig -name "*.cs")

$(XMLCONFIG): $(XMLCONFIG_SRC)
	gmcs -out:$(XMLCONFIG) -target:library -unsafe+ $(TRACE_FLAG) $(XMLCONFIG_SRC) -r:$(MUSIC_BIN)

configs: $(XMLCONFIG)

#############
### MUSIC ###
#############

MUSIC_BIN=bin/musiC.dll
MUSIC_SRC=$(shell find ./src/MusiC -name "*.cs")

$(MUSIC_BIN): $(MUSIC_SRC)
	gmcs -unsafe+ -out:$(MUSIC_BIN) -target:library $(TRACE_FLAG) $(MUSIC_SRC) -r:bin/log4net.dll

musiC: $(MUSIC_BIN)

####################
### APPLICATIONS ###
####################

GENREC_BIN=bin/GenreC.exe
GENREC_SRC=$(shell find ./src/Apps/GenreC -name "*.cs")

$(GENREC_BIN):$(GENREC_SRC) $(MUSIC_BIN)
	gmcs -unsafe+ -out:$(GENREC_BIN) -target:exe $(TRACE_FLAG) $(GENREC_SRC) -r:$(MUSIC_BIN)
	
applications: $(GENREC_BIN)

##################
### DEPENDENCY ###
##################

UDEPS_FULL=$(shell find $(DEPS_BIN_DIR) -name "*.*")
UDEPS=$(addprefix ./bin/,$(notdir $(DEPS_FULL)))

$(UDEPS):
	cp $(addprefix $(DEPS_BIN_DIR)/, $(notdir $@)) bin

MDEPS_FULL=$(shell find $(MNG_DEPS_BIN_DIR) -name "*.*")
MDEPS=$(addprefix ./bin/,$(notdir $(MDEPS_FULL)))

$(MDEPS):
	cp $(addprefix $(MNG_DEPS_BIN_DIR)/, $(notdir $@)) bin

MUSIC_CONFIG_SOURCE=src/MusiC/MusiC.log.config
MUSIC_CONFIG=bin/MusiC.log.config

$(MUSIC_CONFIG): $(MUSIC_CONFIG_SOURCE)
	cp $(MUSIC_CONFIG_SOURCE) bin
	
dependency: $(UDEPS) $(MDEPS) $(MUSIC_CONFIG)

################################################################################

#####################
### DOCUMENTATION ###
#####################

srcDoc/html/index.html: extensions musiC applications
	/Applications/Doxygen.app/Contents/Resources/doxygen mac.dox2
	/Applications/Firefox.app/Contents/MacOS/firefox $@

.PHONY=srcDoc
srcDoc: srcDoc/html/index.html
