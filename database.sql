--
-- PostgreSQL database dump
--

-- Dumped from database version 13.8 (Debian 13.8-1.pgdg110+1)
-- Dumped by pg_dump version 14.3

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

--
-- Name: ArtifactStatus; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public."ArtifactStatus" AS ENUM (
    'PROCESSING',
    'PROCESSED'
);


ALTER TYPE public."ArtifactStatus" OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: artifact_dependencies; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.artifact_dependencies (
    id integer NOT NULL,
    artifact_id integer NOT NULL,
    name character varying NOT NULL
);


ALTER TABLE public.artifact_dependencies OWNER TO postgres;

--
-- Name: ArtifactDependencies_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."ArtifactDependencies_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public."ArtifactDependencies_Id_seq" OWNER TO postgres;

--
-- Name: ArtifactDependencies_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."ArtifactDependencies_Id_seq" OWNED BY public.artifact_dependencies.id;


--
-- Name: artifact_versions; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.artifact_versions (
    id integer NOT NULL,
    artifact_id integer NOT NULL,
    version character varying NOT NULL,
    location character varying NOT NULL,
    status character varying NOT NULL
);


ALTER TABLE public.artifact_versions OWNER TO postgres;

--
-- Name: ArtifactVersions_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."ArtifactVersions_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public."ArtifactVersions_Id_seq" OWNER TO postgres;

--
-- Name: ArtifactVersions_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."ArtifactVersions_Id_seq" OWNED BY public.artifact_versions.id;


--
-- Name: artifacts; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.artifacts (
    id integer NOT NULL,
    name character varying NOT NULL,
    module character varying NOT NULL,
    status character varying NOT NULL,
    root boolean DEFAULT false NOT NULL
);


ALTER TABLE public.artifacts OWNER TO postgres;

--
-- Name: Artifacts_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Artifacts_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public."Artifacts_Id_seq" OWNER TO postgres;

--
-- Name: Artifacts_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Artifacts_Id_seq" OWNED BY public.artifacts.id;


--
-- Name: artifact_dependencies id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.artifact_dependencies ALTER COLUMN id SET DEFAULT nextval('public."ArtifactDependencies_Id_seq"'::regclass);


--
-- Name: artifact_versions id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.artifact_versions ALTER COLUMN id SET DEFAULT nextval('public."ArtifactVersions_Id_seq"'::regclass);


--
-- Name: artifacts id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.artifacts ALTER COLUMN id SET DEFAULT nextval('public."Artifacts_Id_seq"'::regclass);


--
-- Name: artifact_dependencies ArtifactDependencies_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.artifact_dependencies
    ADD CONSTRAINT "ArtifactDependencies_pkey" PRIMARY KEY (id);


--
-- Name: artifact_versions ArtifactVersions_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.artifact_versions
    ADD CONSTRAINT "ArtifactVersions_pkey" PRIMARY KEY (id);


--
-- Name: artifacts Artifacts_Name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.artifacts
    ADD CONSTRAINT "Artifacts_Name_key" UNIQUE (name);


--
-- Name: artifacts Artifacts_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.artifacts
    ADD CONSTRAINT "Artifacts_pkey" PRIMARY KEY (id);


--
-- Name: fki_ArtifactId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "fki_ArtifactId" ON public.artifact_versions USING btree (artifact_id);


--
-- Name: artifact_versions ArtifactId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.artifact_versions
    ADD CONSTRAINT "ArtifactId" FOREIGN KEY (artifact_id) REFERENCES public.artifacts(id) ON DELETE CASCADE NOT VALID;


--
-- Name: artifact_dependencies ArtifactId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.artifact_dependencies
    ADD CONSTRAINT "ArtifactId" FOREIGN KEY (artifact_id) REFERENCES public.artifacts(id) ON DELETE CASCADE NOT VALID;


--
-- PostgreSQL database dump complete
--

