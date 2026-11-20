#include <glad/glad.h>
#include <SDL3/SDL.h>
#include <iostream>
#include <fstream>
#include <sstream>

/** lê o arquivo do shader e devolve o código como string */
std::string loadShaderSource(const char* filePath) {
    std::ifstream shaderFile(filePath);
    if (!shaderFile.is_open()) {
        std::cerr << "Erro ao abrir " << filePath << std::endl;
        exit(EXIT_FAILURE);
    }
    std::stringstream shaderStream;
    shaderStream << shaderFile.rdbuf();
    return shaderStream.str();
}

/** compila um shader (vertex ou fragment) e mostra no terminal se der erro */
GLuint compileShader(GLenum type, const char* filePath) {
    std::string code = loadShaderSource(filePath);
    const char* shaderCode = code.c_str();

    GLuint shader = glCreateShader(type);
    glShaderSource(shader, 1, &shaderCode, nullptr);
    glCompileShader(shader);

    GLint success;
    glGetShaderiv(shader, GL_COMPILE_STATUS, &success);
    if (!success) {
        char infoLog[512];
        glGetShaderInfoLog(shader, 512, nullptr, infoLog);
        std::cerr << "Erro ao compilar " << filePath << ":\n" << infoLog << std::endl;
    }

    return shader;
}

/** compila os dois shaders e junta num programa só */
GLuint createShaderProgram(const char* vertexPath, const char* fragmentPath) {
    GLuint vertexShader = compileShader(GL_VERTEX_SHADER, vertexPath);
    GLuint fragmentShader = compileShader(GL_FRAGMENT_SHADER, fragmentPath);

    GLuint shaderProgram = glCreateProgram();
    glAttachShader(shaderProgram, vertexShader);
    glAttachShader(shaderProgram, fragmentShader);
    glLinkProgram(shaderProgram);

    GLint success;
    glGetProgramiv(shaderProgram, GL_LINK_STATUS, &success);
    if (!success) {
        char infoLog[512];
        glGetProgramInfoLog(shaderProgram, 512, nullptr, infoLog);
        std::cerr << "Erro ao linkar programa:\n" << infoLog << std::endl;
    }

    glDeleteShader(vertexShader);
    glDeleteShader(fragmentShader);

    return shaderProgram;
}

int main() {
    // === Inicializa SDL ===
    if (!SDL_Init(SDL_INIT_VIDEO)) {
        std::cerr << "Erro ao inicializar SDL: " << SDL_GetError() << std::endl;
        return -1;
    }

    SDL_GL_SetAttribute(SDL_GL_CONTEXT_MAJOR_VERSION, 3);
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_MINOR_VERSION, 3);
    SDL_GL_SetAttribute(SDL_GL_CONTEXT_PROFILE_MASK, SDL_GL_CONTEXT_PROFILE_CORE);

    const int width = 800, height = 600;
    SDL_Window* window = SDL_CreateWindow("Image Shader", width, height, SDL_WINDOW_OPENGL);
    if (!window) {
        std::cerr << "Erro ao criar janela SDL: " << SDL_GetError() << std::endl;
        SDL_Quit();
        return -1;
    }

    // cria o contexto opengl associado à janela
    SDL_GLContext context = SDL_GL_CreateContext(window);
    if (!context) {
        std::cerr << "Erro ao criar contexto OpenGL: " << SDL_GetError() << std::endl;
        SDL_Quit();
        return -1;
    }

    // Isso ativa vsync pra poupar uso de GPU e economizar bateria dos notebooks dos estudantes, porque um programa leve assim vai rodar a milhares de fps.
    SDL_GL_SetSwapInterval(1);

    if (!gladLoadGLLoader((GLADloadproc)SDL_GL_GetProcAddress)) {
        std::cerr << "Erro ao inicializar GLAD" << std::endl;
        return -1;
    }

    // === Cria quad de tela inteira ===
    float vertices[] = {
        // positions        // colors         // texcoords
        -1.0f, -1.0f, 0.0f,  1.0f,0.0f,0.0f,  0.0f,0.0f,
         1.0f, -1.0f, 0.0f,  0.0f,1.0f,0.0f,  1.0f,0.0f,
         1.0f,  1.0f, 0.0f,  0.0f,0.0f,1.0f,  1.0f,1.0f,
        -1.0f,  1.0f, 0.0f,  1.0f,1.0f,1.0f,  0.0f,1.0f
    };
    unsigned int indices[] = {
        0, 1, 2,
        2, 3, 0
    };

    unsigned int VAO, VBO, EBO;
    glGenVertexArrays(1, &VAO);
    glGenBuffers(1, &VBO);
    glGenBuffers(1, &EBO);

    glBindVertexArray(VAO);
    glBindBuffer(GL_ARRAY_BUFFER, VBO);
    glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
    glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(indices), indices, GL_STATIC_DRAW);

    glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (void*)0);
    glEnableVertexAttribArray(0);
    glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (void*)(3 * sizeof(float)));
    glEnableVertexAttribArray(1);
    glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (void*)(6 * sizeof(float)));
    glEnableVertexAttribArray(2);

    // === Cria e usa o programa de shader ===
    GLuint shaderProgram = createShaderProgram("Shaders/VertexShader.vs", "Shaders/Image.fs");
    glUseProgram(shaderProgram);

    // === Loop principal ===
    bool running = true;
    while (running) {
        // consome eventos e trata o fechamento
        SDL_Event event;
        while (SDL_PollEvent(&event)) {
            if (event.type == SDL_EVENT_QUIT) {
                running = false;
            }
        }

        // Define cor de fundo
        glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        glClear(GL_COLOR_BUFFER_BIT);

        // Atualiza uniforms
        float timeValue = SDL_GetTicks() / 1000.0f; // milissegundos -> segundos
        int w, h;
        SDL_GetWindowSizeInPixels(window, &w, &h);
        float mouseX, mouseY;
        SDL_GetMouseState(&mouseX, &mouseY);

        glUseProgram(shaderProgram);
        glUniform1f(glGetUniformLocation(shaderProgram, "iTime"), timeValue);
        glUniform2f(glGetUniformLocation(shaderProgram, "iResolution"), (float)w, (float)h);
        glUniform4f(glGetUniformLocation(shaderProgram, "iMouse"), (float)mouseX, (float)mouseY, 0.0f, 0.0f);

        // Desenha o quad
        glBindVertexArray(VAO);
        glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);

        // Atualiza a janela
        SDL_GL_SwapWindow(window);
    }

    // === Limpeza ===
    glDeleteVertexArrays(1, &VAO);
    glDeleteBuffers(1, &VBO);
    glDeleteBuffers(1, &EBO);
    SDL_GL_DestroyContext(context);
    SDL_DestroyWindow(window);
    SDL_Quit();

    return 0;
}
