cmake_minimum_required(VERSION 3.16)

set (CMAKE_CXX_STANDARD 17)

project(Barycentric)
configure_file(${PROJECT_SOURCE_DIR}/configuration/root_directory.h.in configuration/root_directory.h)
include_directories(${CMAKE_BINARY_DIR}/configuration)
include_directories( ${PROJECT_SOURCE_DIR}/includes )

# Baixa e compila o SDL3 junto com o projeto (não precisa instalar no sistema)
include(FetchContent)
FetchContent_Declare(
    SDL3
    GIT_REPOSITORY https://github.com/libsdl-org/SDL.git
    GIT_TAG        release-3.4.20
    GIT_SHALLOW    TRUE
)
FetchContent_MakeAvailable(SDL3)

add_executable(Barycentric stb_image/stb_image.cpp Shader.h glad.c Barycentric.cpp)

target_link_libraries(Barycentric GL dl SDL3::SDL3)
