CREATE TABLE Usuario (
    id_usuario SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    correo VARCHAR(150) UNIQUE NOT NULL,
    nombre_usuario VARCHAR(100) UNIQUE NOT NULL,
    rol VARCHAR(50) NOT NULL,
    contrasenia VARCHAR(255) NOT NULL,
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE Rodeo (
    id_rodeo SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE Animal (
    id_animal SERIAL PRIMARY KEY,
    caravana_cuig VARCHAR(5) NOT NULL,
    caravana_nro_manejo VARCHAR(5) NOT NULL,
    fecha_nacimiento DATE NOT NULL,
    peso_al_nacer DECIMAL NOT NULL,
    raza VARCHAR(100) NOT NULL,
    sexo VARCHAR(10) NOT NULL,
    color_pelaje VARCHAR(100),
    fecha_alta DATE NOT NULL,
    estado BOOLEAN NOT NULL,
    id_madre INTEGER REFERENCES Animal(id_animal),
    id_padre INTEGER REFERENCES Animal(id_animal),
    id_rodeo INTEGER REFERENCES Rodeo(id_rodeo),
    UNIQUE (caravana_cuig, caravana_nro_manejo)
);

CREATE TABLE Medicamento (
    id_medicamento SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    activo BOOLEAN NOT NULL DEFAULT true
);

CREATE TABLE EventoSanitario (
    id_evento_sanitario SERIAL PRIMARY KEY,
    tipo_evento VARCHAR(50) NOT NULL,
    fecha_evento DATE NOT NULL,
    vigencia_hasta DATE,
    fecha_proxima_aplicacion DATE,
    observaciones VARCHAR(500),
    id_usuario INTEGER REFERENCES Usuario(id_usuario) ON DELETE RESTRICT NOT NULL,
    id_animal INTEGER REFERENCES Animal(id_animal) ON DELETE RESTRICT NOT NULL
);

CREATE TABLE DetalleMedicamento (
    id_detalle_medicamento SERIAL PRIMARY KEY,
    dosis DECIMAL,
    unidad VARCHAR(10),
    observaciones VARCHAR(500),
    id_evento_sanitario INTEGER REFERENCES EventoSanitario(id_evento_sanitario) ON DELETE CASCADE NOT NULL,
    id_medicamento INTEGER REFERENCES Medicamento(id_medicamento) ON DELETE RESTRICT NOT NULL
);

CREATE TABLE ControlDePeso (
    id_control_de_peso SERIAL PRIMARY KEY,
    fecha_pesaje DATE NOT NULL,
    peso_kg DECIMAL NOT NULL,
    observaciones TEXT,
    id_usuario INTEGER REFERENCES Usuario(id_usuario) NOT NULL,
    id_animal INTEGER REFERENCES Animal(id_animal) NOT NULL
);

CREATE TABLE Ingrediente (
    id_ingrediente SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    minerales TEXT,
    energia_metabolizable DECIMAL,
    proteina_bruta DECIMAL,
    fibra_det_neutro DECIMAL,
    unidad_medida VARCHAR(50) NOT NULL,
    aditivos TEXT,
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE PlanAlimenticio (
    id_plan_alimenticio SERIAL PRIMARY KEY,
    nombre_plan VARCHAR(100) NOT NULL UNIQUE,
    categoria VARCHAR(50),
    peso_vivo_inicial_promedio DECIMAL,
    peso_objetivo DECIMAL,
    ganancia_peso_esperada DECIMAL,
    tipo_alimentacion VARCHAR(50) NOT NULL,
    tiempo_alimentacion VARCHAR(100),
    kg_ms_diaria_por_animal DECIMAL NOT NULL,
    observaciones TEXT
);

CREATE TABLE PlanAlimenticioDetalle (
    id_plan_alimenticio_detalle SERIAL PRIMARY KEY,
    porcentaje_inclusion_ms DECIMAL NOT NULL,
    observaciones TEXT,
    id_plan_alimenticio INTEGER REFERENCES PlanAlimenticio(id_plan_alimenticio) NOT NULL,
    id_ingrediente INTEGER REFERENCES Ingrediente(id_ingrediente) NOT NULL
);

CREATE TABLE PlanRodeoAsignacion (
    id_asignacion_rodeo SERIAL PRIMARY KEY,
    vigencia_desde DATE NOT NULL,
    vigencia_hasta DATE,
    activo BOOLEAN NOT NULL DEFAULT TRUE,
    id_plan_alimenticio INTEGER REFERENCES PlanAlimenticio(id_plan_alimenticio) NOT NULL,
    id_rodeo INTEGER REFERENCES Rodeo(id_rodeo) NOT NULL
);

CREATE UNIQUE INDEX ux_rodeo_nombre_activo ON rodeo (LOWER(nombre)) WHERE activo;

CREATE UNIQUE INDEX ux_asignacion_rodeo_activa ON planrodeoasignacion (id_rodeo) WHERE activo;

CREATE UNIQUE INDEX ux_detalle_plan_ingrediente ON planalimenticiodetalle (id_plan_alimenticio, id_ingrediente);

CREATE UNIQUE INDEX ux_medicamento_nombre ON medicamento (LOWER(nombre));

CREATE UNIQUE INDEX ux_ingrediente_nombre_activo ON ingrediente (LOWER(nombre)) WHERE activo;