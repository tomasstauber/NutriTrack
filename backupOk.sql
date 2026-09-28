--
-- PostgreSQL database dump
--

\restrict 3OxiX7p7jXci8kbpUJBahTPxiPJ7WTvcFRLit5gxuky83Zg9PigmxaTPe9kubVe

-- Dumped from database version 16.13 (Debian 16.13-1.pgdg13+1)
-- Dumped by pg_dump version 16.13 (Debian 16.13-1.pgdg13+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: animal; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.animal (
    id_animal integer NOT NULL,
    caravana_cuig character varying(5) NOT NULL,
    caravana_nro_manejo character varying(5) NOT NULL,
    fecha_nacimiento date,
    peso_al_nacer numeric,
    id_madre integer,
    id_padre integer,
    raza character varying(100),
    sexo character varying(10),
    color_pelaje character varying(100),
    fecha_alta date,
    estado boolean,
    id_rodeo integer
);


ALTER TABLE public.animal OWNER TO postgres;

--
-- Name: animal_id_animal_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.animal_id_animal_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.animal_id_animal_seq OWNER TO postgres;

--
-- Name: animal_id_animal_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.animal_id_animal_seq OWNED BY public.animal.id_animal;


--
-- Name: controldepeso; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.controldepeso (
    id_control_de_peso integer NOT NULL,
    fecha_pesaje date,
    peso_kg numeric,
    observaciones text,
    id_usuario integer,
    id_animal integer
);


ALTER TABLE public.controldepeso OWNER TO postgres;

--
-- Name: controldepeso_id_control_de_peso_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.controldepeso_id_control_de_peso_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.controldepeso_id_control_de_peso_seq OWNER TO postgres;

--
-- Name: controldepeso_id_control_de_peso_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.controldepeso_id_control_de_peso_seq OWNED BY public.controldepeso.id_control_de_peso;


--
-- Name: detallemedicamento; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.detallemedicamento (
    id_detalle_medicamento integer NOT NULL,
    dosis numeric,
    unidad character varying(10),
    id_evento_sanitario integer,
    id_medicamento integer
);


ALTER TABLE public.detallemedicamento OWNER TO postgres;

--
-- Name: detallemedicamento_id_detalle_medicamento_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.detallemedicamento_id_detalle_medicamento_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.detallemedicamento_id_detalle_medicamento_seq OWNER TO postgres;

--
-- Name: detallemedicamento_id_detalle_medicamento_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.detallemedicamento_id_detalle_medicamento_seq OWNED BY public.detallemedicamento.id_detalle_medicamento;


--
-- Name: eventosanitario; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.eventosanitario (
    id_evento_sanitario integer NOT NULL,
    tipo_de_evento character varying(50),
    vigencia_hasta date,
    fecha_evento date,
    fecha_proxima_aplicacion date,
    id_usuario integer,
    id_animal integer
);


ALTER TABLE public.eventosanitario OWNER TO postgres;

--
-- Name: eventosanitario_id_evento_sanitario_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.eventosanitario_id_evento_sanitario_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.eventosanitario_id_evento_sanitario_seq OWNER TO postgres;

--
-- Name: eventosanitario_id_evento_sanitario_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.eventosanitario_id_evento_sanitario_seq OWNED BY public.eventosanitario.id_evento_sanitario;


--
-- Name: ingrediente; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.ingrediente (
    id_ingrediente integer NOT NULL,
    nombre character varying(100) NOT NULL,
    descripcion text,
    minerales text,
    energia_metabolizable numeric,
    proteina_bruta numeric,
    fibra_det_neutro numeric,
    unidad_medida character varying(50),
    aditivos text,
    activo boolean DEFAULT true NOT NULL
);


ALTER TABLE public.ingrediente OWNER TO postgres;

--
-- Name: ingrediente_id_ingrediente_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.ingrediente_id_ingrediente_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.ingrediente_id_ingrediente_seq OWNER TO postgres;

--
-- Name: ingrediente_id_ingrediente_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.ingrediente_id_ingrediente_seq OWNED BY public.ingrediente.id_ingrediente;


--
-- Name: medicamento; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.medicamento (
    id_medicamento integer NOT NULL,
    nombre character varying(100) NOT NULL,
    descripcion text,
    activo boolean DEFAULT true NOT NULL
);


ALTER TABLE public.medicamento OWNER TO postgres;

--
-- Name: medicamento_id_medicamento_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.medicamento_id_medicamento_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.medicamento_id_medicamento_seq OWNER TO postgres;

--
-- Name: medicamento_id_medicamento_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.medicamento_id_medicamento_seq OWNED BY public.medicamento.id_medicamento;


--
-- Name: planalimenticio; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.planalimenticio (
    id_plan_alimenticio integer NOT NULL,
    nombre_plan character varying(100) NOT NULL,
    categoria character varying(50),
    peso_vivo_inicial_promedio numeric,
    peso_objetivo numeric,
    ganancia_peso_esperada numeric,
    tipo_alimentacion character varying(50) NOT NULL,
    tiempo_alimentacion character varying(100),
    kg_ms_diaria_por_animal numeric NOT NULL,
    observaciones text
);


ALTER TABLE public.planalimenticio OWNER TO postgres;

--
-- Name: planalimenticio_id_plan_alimenticio_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.planalimenticio_id_plan_alimenticio_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.planalimenticio_id_plan_alimenticio_seq OWNER TO postgres;

--
-- Name: planalimenticio_id_plan_alimenticio_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.planalimenticio_id_plan_alimenticio_seq OWNED BY public.planalimenticio.id_plan_alimenticio;


--
-- Name: planalimenticiodetalle; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.planalimenticiodetalle (
    id_plan_alimenticio_detalle integer NOT NULL,
    porcentaje_inclusion_ms numeric,
    observaciones text,
    id_plan_alimenticio integer,
    id_ingrediente integer
);


ALTER TABLE public.planalimenticiodetalle OWNER TO postgres;

--
-- Name: planalimenticiodetalle_id_plan_alimenticio_detalle_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.planalimenticiodetalle_id_plan_alimenticio_detalle_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.planalimenticiodetalle_id_plan_alimenticio_detalle_seq OWNER TO postgres;

--
-- Name: planalimenticiodetalle_id_plan_alimenticio_detalle_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.planalimenticiodetalle_id_plan_alimenticio_detalle_seq OWNED BY public.planalimenticiodetalle.id_plan_alimenticio_detalle;


--
-- Name: planrodeoasignacion; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.planrodeoasignacion (
    id_asignacion_rodeo integer NOT NULL,
    vigencia_desde date NOT NULL,
    vigencia_hasta date,
    activo boolean DEFAULT true NOT NULL,
    id_plan_alimenticio integer,
    id_rodeo integer
);


ALTER TABLE public.planrodeoasignacion OWNER TO postgres;

--
-- Name: planrodeoasignacion_id_asignacion_rodeo_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.planrodeoasignacion_id_asignacion_rodeo_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.planrodeoasignacion_id_asignacion_rodeo_seq OWNER TO postgres;

--
-- Name: planrodeoasignacion_id_asignacion_rodeo_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.planrodeoasignacion_id_asignacion_rodeo_seq OWNED BY public.planrodeoasignacion.id_asignacion_rodeo;


--
-- Name: rodeo; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.rodeo (
    id_rodeo integer NOT NULL,
    nombre character varying(100) NOT NULL,
    descripcion text
);


ALTER TABLE public.rodeo OWNER TO postgres;

--
-- Name: rodeo_id_rodeo_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.rodeo_id_rodeo_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.rodeo_id_rodeo_seq OWNER TO postgres;

--
-- Name: rodeo_id_rodeo_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.rodeo_id_rodeo_seq OWNED BY public.rodeo.id_rodeo;


--
-- Name: usuario; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.usuario (
    id_usuario integer NOT NULL,
    nombre character varying(100) NOT NULL,
    correo character varying(150) NOT NULL,
    rol character varying(50) NOT NULL,
    contrasenia character varying(255) NOT NULL
);


ALTER TABLE public.usuario OWNER TO postgres;

--
-- Name: usuario_id_usuario_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.usuario_id_usuario_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.usuario_id_usuario_seq OWNER TO postgres;

--
-- Name: usuario_id_usuario_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.usuario_id_usuario_seq OWNED BY public.usuario.id_usuario;


--
-- Name: animal id_animal; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.animal ALTER COLUMN id_animal SET DEFAULT nextval('public.animal_id_animal_seq'::regclass);


--
-- Name: controldepeso id_control_de_peso; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.controldepeso ALTER COLUMN id_control_de_peso SET DEFAULT nextval('public.controldepeso_id_control_de_peso_seq'::regclass);


--
-- Name: detallemedicamento id_detalle_medicamento; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.detallemedicamento ALTER COLUMN id_detalle_medicamento SET DEFAULT nextval('public.detallemedicamento_id_detalle_medicamento_seq'::regclass);


--
-- Name: eventosanitario id_evento_sanitario; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.eventosanitario ALTER COLUMN id_evento_sanitario SET DEFAULT nextval('public.eventosanitario_id_evento_sanitario_seq'::regclass);


--
-- Name: ingrediente id_ingrediente; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingrediente ALTER COLUMN id_ingrediente SET DEFAULT nextval('public.ingrediente_id_ingrediente_seq'::regclass);


--
-- Name: medicamento id_medicamento; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.medicamento ALTER COLUMN id_medicamento SET DEFAULT nextval('public.medicamento_id_medicamento_seq'::regclass);


--
-- Name: planalimenticio id_plan_alimenticio; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.planalimenticio ALTER COLUMN id_plan_alimenticio SET DEFAULT nextval('public.planalimenticio_id_plan_alimenticio_seq'::regclass);


--
-- Name: planalimenticiodetalle id_plan_alimenticio_detalle; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.planalimenticiodetalle ALTER COLUMN id_plan_alimenticio_detalle SET DEFAULT nextval('public.planalimenticiodetalle_id_plan_alimenticio_detalle_seq'::regclass);


--
-- Name: planrodeoasignacion id_asignacion_rodeo; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.planrodeoasignacion ALTER COLUMN id_asignacion_rodeo SET DEFAULT nextval('public.planrodeoasignacion_id_asignacion_rodeo_seq'::regclass);


--
-- Name: rodeo id_rodeo; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rodeo ALTER COLUMN id_rodeo SET DEFAULT nextval('public.rodeo_id_rodeo_seq'::regclass);


--
-- Name: usuario id_usuario; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuario ALTER COLUMN id_usuario SET DEFAULT nextval('public.usuario_id_usuario_seq'::regclass);


--
-- Data for Name: animal; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.animal (id_animal, caravana_cuig, caravana_nro_manejo, fecha_nacimiento, peso_al_nacer, id_madre, id_padre, raza, sexo, color_pelaje, fecha_alta, estado, id_rodeo) FROM stdin;
1	AR001	00001	2019-03-15	38.0	\N	\N	Angus	Hembra	negro	2019-03-15	t	1
2	AR001	00002	2018-06-20	42.0	\N	\N	Hereford	Macho	colorado	2018-06-20	t	1
3	AR002	00003	2020-01-10	36.0	\N	\N	Angus	Hembra	negro	2020-01-10	t	2
4	AR002	00004	2017-09-05	45.0	\N	\N	Brahman	Macho	gris	2017-09-05	t	2
5	AR003	00005	2022-08-12	34.0	1	2	Angus	Hembra	negro	2022-08-12	t	\N
6	AR003	00006	2023-02-28	37.0	3	4	Brahman	Macho	gris	2023-02-28	t	\N
\.


--
-- Data for Name: controldepeso; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.controldepeso (id_control_de_peso, fecha_pesaje, peso_kg, observaciones, id_usuario, id_animal) FROM stdin;
1	2024-01-15	320.5	Animal en buen estado	3	1
2	2024-01-15	480.0	\N	3	2
3	2024-02-20	295.0	Leve baja de peso, monitorear	3	3
4	2024-03-10	510.0	\N	3	4
5	2024-04-05	180.0	Ternera en desarrollo normal	3	5
\.


--
-- Data for Name: detallemedicamento; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.detallemedicamento (id_detalle_medicamento, dosis, unidad, id_evento_sanitario, id_medicamento) FROM stdin;
1	5.0	ml	1	4
2	2.5	ml	2	1
3	10.0	ml	3	4
4	15.0	ml	4	2
\.


--
-- Data for Name: eventosanitario; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.eventosanitario (id_evento_sanitario, tipo_de_evento, vigencia_hasta, fecha_evento, fecha_proxima_aplicacion, id_usuario, id_animal) FROM stdin;
1	vacunacion	2025-06-01	2024-06-01	2025-06-01	2	1
2	desparasitacion	2024-12-01	2024-06-01	2024-12-01	2	2
3	vacunacion	2025-06-15	2024-06-15	2025-06-15	2	3
4	antibiotico	\N	2024-07-10	\N	2	4
\.


--
-- Data for Name: ingrediente; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingrediente (id_ingrediente, nombre, descripcion, minerales, energia_metabolizable, proteina_bruta, fibra_det_neutro, unidad_medida, aditivos, activo) FROM stdin;
2	Heno de alfalfa	Alfalfa de primer corte	Calcio	2.1	18.0	35.0	kg	\N	t
1	Maíz molido	Grano de maíz molido grueso	Calcio, Fósforo	3.2	8.5	9.0	kg	\N	t
3	Sorgo granífero	Sorgo de alta energía	Magnesio	3.0	9.0	8.5	kg	\N	t
4	Núcleo mineral	Premezcla mineral para bovinos	Calcio, Fósforo, Magnesio, Zinc	0.0	0.0	0.0	kg	antioxidante	t
\.


--
-- Data for Name: medicamento; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.medicamento (id_medicamento, nombre, descripcion, activo) FROM stdin;
1	Ivermectina	Antiparasitario de amplio espectro	t
4	Vacuna Aftosa	Vacuna bivalente contra fiebre aftosa	t
2	Oxitetraciclina	Antibiótico de amplio espectro	t
3	Vitamina AD3E	Complejo vitamínico	t
\.


--
-- Data for Name: planalimenticio; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.planalimenticio (id_plan_alimenticio, nombre_plan, categoria, peso_vivo_inicial_promedio, peso_objetivo, ganancia_peso_esperada, tipo_alimentacion, tiempo_alimentacion, kg_ms_diaria_por_animal, observaciones) FROM stdin;
1	Plan engorde novillos Q1	novillo	350.0	480.0	0.8	corral	180 dias	12.0	Plan engorde primer semestre
2	Plan mantenimiento vacas	vaca	420.0	450.0	0.3	pastura implantada	365 dias	8.0	Plan mantenimiento vacas
3	Plan destete terneros	ternero	80.0	180.0	0.6	campo natural	180 dias	4.0	Plan destete y desarrollo
\.


--
-- Data for Name: planalimenticiodetalle; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.planalimenticiodetalle (id_plan_alimenticio_detalle, porcentaje_inclusion_ms, observaciones, id_plan_alimenticio, id_ingrediente) FROM stdin;
2	30.0	\N	1	2
3	10.0	Aporte mineral	1	4
4	50.0	\N	2	2
5	50.0	\N	2	3
6	70.0	Base principal del plan	3	1
7	30.0	Aporte mineral	3	4
1	60.0	Base energética del plan	1	1
\.


--
-- Data for Name: planrodeoasignacion; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.planrodeoasignacion (id_asignacion_rodeo, vigencia_desde, vigencia_hasta, activo, id_plan_alimenticio, id_rodeo) FROM stdin;
1	2024-01-01	2024-06-30	t	1	1
2	2024-01-01	2024-12-31	t	2	2
3	2024-03-01	2024-09-30	t	3	3
\.


--
-- Data for Name: rodeo; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.rodeo (id_rodeo, nombre, descripcion) FROM stdin;
1	Rodeo Norte	Rodeo ubicado en el sector norte de la estancia
2	Rodeo Sur	Rodeo ubicado en el sector sur de la estancia
3	Rodeo Central	Rodeo principal de la estancia
\.


--
-- Data for Name: usuario; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.usuario (id_usuario, nombre, correo, rol, contrasenia) FROM stdin;
1	Juan Pérez	juan@estancia.com	administrador	$2b$10$hashadmin123
2	María Gómez	maria@estancia.com	asesor técnico	$2b$10$hashvet456
3	Carlos López	carlos@estancia.com	encargado de campo	$2b$10$hashop789
\.


--
-- Name: animal_id_animal_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.animal_id_animal_seq', 6, true);


--
-- Name: controldepeso_id_control_de_peso_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.controldepeso_id_control_de_peso_seq', 5, true);


--
-- Name: detallemedicamento_id_detalle_medicamento_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.detallemedicamento_id_detalle_medicamento_seq', 4, true);


--
-- Name: eventosanitario_id_evento_sanitario_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.eventosanitario_id_evento_sanitario_seq', 4, true);


--
-- Name: ingrediente_id_ingrediente_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingrediente_id_ingrediente_seq', 4, true);


--
-- Name: medicamento_id_medicamento_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.medicamento_id_medicamento_seq', 4, true);


--
-- Name: planalimenticio_id_plan_alimenticio_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.planalimenticio_id_plan_alimenticio_seq', 3, true);


--
-- Name: planalimenticiodetalle_id_plan_alimenticio_detalle_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.planalimenticiodetalle_id_plan_alimenticio_detalle_seq', 7, true);


--
-- Name: planrodeoasignacion_id_asignacion_rodeo_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.planrodeoasignacion_id_asignacion_rodeo_seq', 3, true);


--
-- Name: rodeo_id_rodeo_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.rodeo_id_rodeo_seq', 3, true);


--
-- Name: usuario_id_usuario_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.usuario_id_usuario_seq', 3, true);


--
-- Name: animal animal_caravana_cuig_caravana_nro_manejo_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.animal
    ADD CONSTRAINT animal_caravana_cuig_caravana_nro_manejo_key UNIQUE (caravana_cuig, caravana_nro_manejo);


--
-- Name: animal animal_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.animal
    ADD CONSTRAINT animal_pkey PRIMARY KEY (id_animal);


--
-- Name: controldepeso controldepeso_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.controldepeso
    ADD CONSTRAINT controldepeso_pkey PRIMARY KEY (id_control_de_peso);


--
-- Name: detallemedicamento detallemedicamento_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.detallemedicamento
    ADD CONSTRAINT detallemedicamento_pkey PRIMARY KEY (id_detalle_medicamento);


--
-- Name: eventosanitario eventosanitario_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.eventosanitario
    ADD CONSTRAINT eventosanitario_pkey PRIMARY KEY (id_evento_sanitario);


--
-- Name: ingrediente ingrediente_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingrediente
    ADD CONSTRAINT ingrediente_pkey PRIMARY KEY (id_ingrediente);


--
-- Name: medicamento medicamento_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.medicamento
    ADD CONSTRAINT medicamento_pkey PRIMARY KEY (id_medicamento);


--
-- Name: planalimenticio planalimenticio_nombre_plan_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.planalimenticio
    ADD CONSTRAINT planalimenticio_nombre_plan_key UNIQUE (nombre_plan);


--
-- Name: planalimenticio planalimenticio_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.planalimenticio
    ADD CONSTRAINT planalimenticio_pkey PRIMARY KEY (id_plan_alimenticio);


--
-- Name: planalimenticiodetalle planalimenticiodetalle_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.planalimenticiodetalle
    ADD CONSTRAINT planalimenticiodetalle_pkey PRIMARY KEY (id_plan_alimenticio_detalle);


--
-- Name: planrodeoasignacion planrodeoasignacion_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.planrodeoasignacion
    ADD CONSTRAINT planrodeoasignacion_pkey PRIMARY KEY (id_asignacion_rodeo);


--
-- Name: rodeo rodeo_nombre_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rodeo
    ADD CONSTRAINT rodeo_nombre_key UNIQUE (nombre);


--
-- Name: rodeo rodeo_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rodeo
    ADD CONSTRAINT rodeo_pkey PRIMARY KEY (id_rodeo);


--
-- Name: usuario usuario_correo_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuario
    ADD CONSTRAINT usuario_correo_key UNIQUE (correo);


--
-- Name: usuario usuario_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuario
    ADD CONSTRAINT usuario_pkey PRIMARY KEY (id_usuario);


--
-- Name: animal animal_id_madre_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.animal
    ADD CONSTRAINT animal_id_madre_fkey FOREIGN KEY (id_madre) REFERENCES public.animal(id_animal);


--
-- Name: animal animal_id_padre_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.animal
    ADD CONSTRAINT animal_id_padre_fkey FOREIGN KEY (id_padre) REFERENCES public.animal(id_animal);


--
-- Name: animal animal_id_rodeo_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.animal
    ADD CONSTRAINT animal_id_rodeo_fkey FOREIGN KEY (id_rodeo) REFERENCES public.rodeo(id_rodeo);


--
-- Name: controldepeso controldepeso_id_animal_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.controldepeso
    ADD CONSTRAINT controldepeso_id_animal_fkey FOREIGN KEY (id_animal) REFERENCES public.animal(id_animal);


--
-- Name: controldepeso controldepeso_id_usuario_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.controldepeso
    ADD CONSTRAINT controldepeso_id_usuario_fkey FOREIGN KEY (id_usuario) REFERENCES public.usuario(id_usuario);


--
-- Name: detallemedicamento detallemedicamento_id_evento_sanitario_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.detallemedicamento
    ADD CONSTRAINT detallemedicamento_id_evento_sanitario_fkey FOREIGN KEY (id_evento_sanitario) REFERENCES public.eventosanitario(id_evento_sanitario);


--
-- Name: detallemedicamento detallemedicamento_id_medicamento_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.detallemedicamento
    ADD CONSTRAINT detallemedicamento_id_medicamento_fkey FOREIGN KEY (id_medicamento) REFERENCES public.medicamento(id_medicamento);


--
-- Name: eventosanitario eventosanitario_id_animal_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.eventosanitario
    ADD CONSTRAINT eventosanitario_id_animal_fkey FOREIGN KEY (id_animal) REFERENCES public.animal(id_animal);


--
-- Name: eventosanitario eventosanitario_id_usuario_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.eventosanitario
    ADD CONSTRAINT eventosanitario_id_usuario_fkey FOREIGN KEY (id_usuario) REFERENCES public.usuario(id_usuario);


--
-- Name: planalimenticiodetalle planalimenticiodetalle_id_ingrediente_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.planalimenticiodetalle
    ADD CONSTRAINT planalimenticiodetalle_id_ingrediente_fkey FOREIGN KEY (id_ingrediente) REFERENCES public.ingrediente(id_ingrediente);


--
-- Name: planalimenticiodetalle planalimenticiodetalle_id_plan_alimenticio_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.planalimenticiodetalle
    ADD CONSTRAINT planalimenticiodetalle_id_plan_alimenticio_fkey FOREIGN KEY (id_plan_alimenticio) REFERENCES public.planalimenticio(id_plan_alimenticio);


--
-- Name: planrodeoasignacion planrodeoasignacion_id_plan_alimenticio_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.planrodeoasignacion
    ADD CONSTRAINT planrodeoasignacion_id_plan_alimenticio_fkey FOREIGN KEY (id_plan_alimenticio) REFERENCES public.planalimenticio(id_plan_alimenticio);


--
-- Name: planrodeoasignacion planrodeoasignacion_id_rodeo_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.planrodeoasignacion
    ADD CONSTRAINT planrodeoasignacion_id_rodeo_fkey FOREIGN KEY (id_rodeo) REFERENCES public.rodeo(id_rodeo);


--
-- PostgreSQL database dump complete
--

\unrestrict 3OxiX7p7jXci8kbpUJBahTPxiPJ7WTvcFRLit5gxuky83Zg9PigmxaTPe9kubVe

