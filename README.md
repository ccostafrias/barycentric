# Barycentric Renderer

Esse repositório visa incluir os arquivo bases e instruir àqueles que pretendem realizar o EP de **MVGA**, ministrado pelo professor Helton Hideraldo Biscaro na **EACH USP**. 

Desenvolvido em **OpenGL**, fazemos uso de computação gráfica para, no fim, renderizar um lindo e belo **triângulo** na tela!

Tal projeto foi pensado para compilar e rodar facilmente em qualquer sistema **Linux** com suporte a OpenGL 3.3 ou superior. Caso esteja usando um sistema operacional diferente, como o **Windows**, minha dica é muito simples: *mude de sistema*!

---

## Como configurar

### 1. Clonar o repositório

Clone o projeto com seguinte comando:

```bash
git clone https://github.com/ccostafrias/barycentric.git
cd barycentric
```

---

### 2. Instalar dependências

Agora, é necessário instalar alguns pacotes, como **OpenGL**, **GLFW**, **GLEW** e outras bibliotecas auxiliares:

```bash
sudo apt-get install g++ cmake git
sudo apt-get install libsoil-dev libglm-dev libassimp-dev libglew-dev libglfw3-dev libxinerama-dev libxcursor-dev libxi-dev libfreetype-dev libgl1-mesa-dev xorg-dev
```

Esses pacotes garantem que tudo irá renderizar perfeitamente.

---

### 3. Compilar o projeto

Para rodar o projeto, iremos rodar no terminal uma série de comandos, que visam:
- limpar qualquer build anterior;
- criar uma nova pasta build;
- compilar e gerar um arquivo binário;
- rodar o arquivo gerado.

```bash
rm -rf build
mkdir build && cd build
cmake ..
cmake --build .
cd ..
./build/Barycentric
```

---

## Objetivo

Agora com o projeto configurado em mãos, o que fazer? O objetivo aqui é gerar um **triângulo** na tela utilizando **coordenadas baricêntricas** e fazê-lo reagir a estímulos do **mouse**, como mudar de cor, piscar, alterar o fundo, aplicar brilho, etc. — vai da sua imaginação! Para tal finalidade, abra o arquivo `Image.fs`, na pasta `Shaders`, e venha compreender mais um pouco da sua estrutura.

---

### 1. Vértices do triângulo

Na função main, estão definidos os três vértices do triângulo (cuja origem é o **centro** da tela):

```cpp
vec2 a = vec2( 0.73,  0.75);
vec2 b = vec2(-0.85,  0.15);
vec2 c = vec2( 0.25, -0.75);
```

Sinta-se livre para modificá-los!

---

### 2. Cálculo das coordenadas baricêntricas

A função `bary()` (ainda **NÃO** implementada) calcula as coordenadas baricêntricas de um ponto em relação aos vértices `a, b, c`.

Essas coordenadas, resumidamente, indicam o quanto o ponto "pertence" a cada vértice, sendo que a **soma** dos componentes desse vetor **DEVE** ser igual a **um** (na dúvida, vale pesquisar e se aprofudnar um pouquinho mais).

```cpp
vec3 bary(in vec3 a, in vec3 b, in vec3 c, in vec3 p) {
    return vec3(0.33, 0.33, 0.33); // exemplo de possível retorno
}
```

---

### 3. Testando pontos

A função `test()` usa `bary()` para determinar se um ponto está dentro ou fora do triângulo — se todas coordenas baricêntricas forem positivas, está **dentro** e retorna `true`; se houver uma negativa, está **fora** e retorna `false`:

```cpp
bool test(in vec2 a, in vec2 b, in vec2 c, in vec2 p, inout vec3 barycoords);
```

Além disso, a função armazena as coordenadas baricêntricas em `barycoords` — um ponteiro (até aqui eles nos perseguem)!.

---

### 4. Renderização na tela

Aqui precisamos entender um breve conceito: a função `main()` passa por cada pixel da tela e no final retornamos sua **cor** no formato **rgba**: red, green, blue, alpha (transparência).

```cpp
FragColor = vec4(color, 1);
```

Portanto, seu objetivo é implementar a lógica da função `bary()`, fazer uma série de testes lógicos na `main()` para decidir qual será a cor final do pixel, modificando `color`! 

Para deixar o efeito final mais interessante, há uma série de funções adicionais (disponibilizadas pelo professor) das quais você pode fazer uso, além da variável `iTime` que possibilita fazer animações (o céu é o limite!).

---

## Conclusão

Obrigado a todos que leram até aqui! Minha intenção nada mais é que ajudar outros colegas que estão (ou estiverão) tão perdidos quanto eu.

Se houver qualquer equívoco, sinta-se livre para mandar um `pull-request` — qualquer contribuição é bem-vinda!

Além disso, o shader original foi deixado na pasta `tests`, caso queira dar uma olhada e comparar.

Enfim, boa sorte a todos e bons experimentos!