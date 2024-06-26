--
-- PostgreSQL database dump
--

-- Dumped from database version 14.5 (Debian 14.5-1.pgdg110+1)
-- Dumped by pg_dump version 15.3 (Ubuntu 15.3-0ubuntu0.23.04.1)

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
-- Name: public; Type: SCHEMA; Schema: -; Owner: postgres
--

-- *not* creating schema, since initdb creates it


ALTER SCHEMA public OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: AspNetRoleClaims; Type: TABLE; Schema: public; Owner: guest
--

CREATE TABLE public."AspNetRoleClaims" (
    "Id" integer NOT NULL,
    "RoleId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


ALTER TABLE public."AspNetRoleClaims" OWNER TO guest;

--
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE; Schema: public; Owner: guest
--

ALTER TABLE public."AspNetRoleClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."AspNetRoleClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: AspNetRoles; Type: TABLE; Schema: public; Owner: guest
--

CREATE TABLE public."AspNetRoles" (
    "Id" text NOT NULL,
    "Name" character varying(256),
    "NormalizedName" character varying(256),
    "ConcurrencyStamp" text
);


ALTER TABLE public."AspNetRoles" OWNER TO guest;

--
-- Name: AspNetUserClaims; Type: TABLE; Schema: public; Owner: guest
--

CREATE TABLE public."AspNetUserClaims" (
    "Id" integer NOT NULL,
    "UserId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


ALTER TABLE public."AspNetUserClaims" OWNER TO guest;

--
-- Name: AspNetUserClaims_Id_seq; Type: SEQUENCE; Schema: public; Owner: guest
--

ALTER TABLE public."AspNetUserClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."AspNetUserClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: AspNetUserLogins; Type: TABLE; Schema: public; Owner: guest
--

CREATE TABLE public."AspNetUserLogins" (
    "LoginProvider" text NOT NULL,
    "ProviderKey" text NOT NULL,
    "ProviderDisplayName" text,
    "UserId" text NOT NULL
);


ALTER TABLE public."AspNetUserLogins" OWNER TO guest;

--
-- Name: AspNetUserRoles; Type: TABLE; Schema: public; Owner: guest
--

CREATE TABLE public."AspNetUserRoles" (
    "UserId" text NOT NULL,
    "RoleId" text NOT NULL
);


ALTER TABLE public."AspNetUserRoles" OWNER TO guest;

--
-- Name: AspNetUserTokens; Type: TABLE; Schema: public; Owner: guest
--

CREATE TABLE public."AspNetUserTokens" (
    "UserId" text NOT NULL,
    "LoginProvider" text NOT NULL,
    "Name" text NOT NULL,
    "Value" text
);


ALTER TABLE public."AspNetUserTokens" OWNER TO guest;

--
-- Name: AspNetUsers; Type: TABLE; Schema: public; Owner: guest
--

CREATE TABLE public."AspNetUsers" (
    "Id" text NOT NULL,
    "FirstName" text,
    "LastName" text,
    "UserName" character varying(256),
    "NormalizedUserName" character varying(256),
    "Email" character varying(256),
    "NormalizedEmail" character varying(256),
    "EmailConfirmed" boolean NOT NULL,
    "PasswordHash" text,
    "SecurityStamp" text,
    "ConcurrencyStamp" text,
    "PhoneNumber" text,
    "PhoneNumberConfirmed" boolean NOT NULL,
    "TwoFactorEnabled" boolean NOT NULL,
    "LockoutEnd" timestamp with time zone,
    "LockoutEnabled" boolean NOT NULL,
    "AccessFailedCount" integer NOT NULL
);


ALTER TABLE public."AspNetUsers" OWNER TO guest;

--
-- Name: RefreshTokens; Type: TABLE; Schema: public; Owner: guest
--

CREATE TABLE public."RefreshTokens" (
    "Id" integer NOT NULL,
    "Token" text,
    "Expires" timestamp with time zone NOT NULL,
    "UserId" integer NOT NULL,
    "RemoteIpAddress" text,
    "Created" timestamp with time zone NOT NULL,
    "Modified" timestamp with time zone NOT NULL
);


ALTER TABLE public."RefreshTokens" OWNER TO guest;

--
-- Name: RefreshTokens_Id_seq; Type: SEQUENCE; Schema: public; Owner: guest
--

ALTER TABLE public."RefreshTokens" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."RefreshTokens_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Users; Type: TABLE; Schema: public; Owner: guest
--

CREATE TABLE public."Users" (
    "Id" integer NOT NULL,
    "IdentityId" text,
    "Address_Street" text,
    "Address_City" text,
    "Address_State" text,
    "Address_Country" text,
    "Address_ZipCode" text,
    "Created" timestamp with time zone NOT NULL,
    "Modified" timestamp with time zone NOT NULL
);


ALTER TABLE public."Users" OWNER TO guest;

--
-- Name: Users_Id_seq; Type: SEQUENCE; Schema: public; Owner: guest
--

ALTER TABLE public."Users" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Users_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: guest
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO guest;

--
-- Data for Name: AspNetRoleClaims; Type: TABLE DATA; Schema: public; Owner: guest
--

COPY public."AspNetRoleClaims" ("Id", "RoleId", "ClaimType", "ClaimValue") FROM stdin;
\.


--
-- Data for Name: AspNetRoles; Type: TABLE DATA; Schema: public; Owner: guest
--

COPY public."AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp") FROM stdin;
\.


--
-- Data for Name: AspNetUserClaims; Type: TABLE DATA; Schema: public; Owner: guest
--

COPY public."AspNetUserClaims" ("Id", "UserId", "ClaimType", "ClaimValue") FROM stdin;
1	8872d9d7-1b0c-4bf1-904b-473dbb122c0f	http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name	khteh
2	44665118-ee61-4797-87b9-228f7f783250	http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name	mickey
\.


--
-- Data for Name: AspNetUserLogins; Type: TABLE DATA; Schema: public; Owner: guest
--

COPY public."AspNetUserLogins" ("LoginProvider", "ProviderKey", "ProviderDisplayName", "UserId") FROM stdin;
\.


--
-- Data for Name: AspNetUserRoles; Type: TABLE DATA; Schema: public; Owner: guest
--

COPY public."AspNetUserRoles" ("UserId", "RoleId") FROM stdin;
\.


--
-- Data for Name: AspNetUserTokens; Type: TABLE DATA; Schema: public; Owner: guest
--

COPY public."AspNetUserTokens" ("UserId", "LoginProvider", "Name", "Value") FROM stdin;
\.


--
-- Data for Name: AspNetUsers; Type: TABLE DATA; Schema: public; Owner: guest
--

COPY public."AspNetUsers" ("Id", "FirstName", "LastName", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount") FROM stdin;
8872d9d7-1b0c-4bf1-904b-473dbb122c0f	Kok How	Teh	khteh	KHTEH	khteh@email.com	KHTEH@EMAIL.COM	f	AQAAAAIAAYagAAAAEOrOL4qBAiNz/1+jRLDGDaTuKmR8BLmCTh3CE8HB4WPnG3szHka1162+gNnhyCg1Dg==	JND2C6QYQARYCYLTSZVUFEH5GGZ6DIKY	88285082-ae9e-469a-a105-e7f838b45454	\N	f	f	\N	t	0
44665118-ee61-4797-87b9-228f7f783250	Mickey	Mouse	mickey	MICKEY	mickey@email.com	MICKEY@EMAIL.COM	f	AQAAAAIAAYagAAAAEHMAkj9J0YykpDyj135EfAu5I6MK1xHfWh+cRdfHVGS+V6RgG1lyr0XTogTNRRtLuw==	MAMLZLV3GCU5TFWRXOI7EYGHA4GXCXHV	6baa75a0-5c39-47cf-ad53-7b4551cbdbcb	\N	f	f	\N	t	0
\.


--
-- Data for Name: RefreshTokens; Type: TABLE DATA; Schema: public; Owner: guest
--

COPY public."RefreshTokens" ("Id", "Token", "Expires", "UserId", "RemoteIpAddress", "Created", "Modified") FROM stdin;
1	b6k2raCCSlFkbWIg9rx2L0qDACPbEHKwn45PcTdb8bE=	2023-03-11 10:38:23.960068+00	1	::1	2023-03-06 10:38:23.976597+00	2023-03-06 10:38:23.976597+00
2	eIWN74FHDuHIzsOjK0kSDbeSSpxZEDWmFpYYHVG30Zo=	2023-03-11 10:38:39.981989+00	2	::1	2023-03-06 10:38:39.982856+00	2023-03-06 10:38:39.982856+00
\.


--
-- Data for Name: Users; Type: TABLE DATA; Schema: public; Owner: guest
--

COPY public."Users" ("Id", "IdentityId", "Address_Street", "Address_City", "Address_State", "Address_Country", "Address_ZipCode", "Created", "Modified") FROM stdin;
1	8872d9d7-1b0c-4bf1-904b-473dbb122c0f	\N	\N	\N	\N	\N	2023-03-06 10:37:54.595493+00	2023-03-06 10:38:23.976598+00
2	44665118-ee61-4797-87b9-228f7f783250	\N	\N	\N	\N	\N	2023-03-06 10:38:10.53502+00	2023-03-06 10:38:39.982857+00
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: guest
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20230306103423_Initial	7.0.3
20230306103432_Initial	7.0.3
20230608071429_UseNotMappedDataAnnotation	7.0.5
\.


--
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: guest
--

SELECT pg_catalog.setval('public."AspNetRoleClaims_Id_seq"', 1, false);


--
-- Name: AspNetUserClaims_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: guest
--

SELECT pg_catalog.setval('public."AspNetUserClaims_Id_seq"', 2, true);


--
-- Name: RefreshTokens_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: guest
--

SELECT pg_catalog.setval('public."RefreshTokens_Id_seq"', 2, true);


--
-- Name: Users_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: guest
--

SELECT pg_catalog.setval('public."Users_Id_seq"', 2, true);


--
-- Name: AspNetRoleClaims PK_AspNetRoleClaims; Type: CONSTRAINT; Schema: public; Owner: guest
--

ALTER TABLE ONLY public."AspNetRoleClaims"
    ADD CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id");


--
-- Name: AspNetRoles PK_AspNetRoles; Type: CONSTRAINT; Schema: public; Owner: guest
--

ALTER TABLE ONLY public."AspNetRoles"
    ADD CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id");


--
-- Name: AspNetUserClaims PK_AspNetUserClaims; Type: CONSTRAINT; Schema: public; Owner: guest
--

ALTER TABLE ONLY public."AspNetUserClaims"
    ADD CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id");


--
-- Name: AspNetUserLogins PK_AspNetUserLogins; Type: CONSTRAINT; Schema: public; Owner: guest
--

ALTER TABLE ONLY public."AspNetUserLogins"
    ADD CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey");


--
-- Name: AspNetUserRoles PK_AspNetUserRoles; Type: CONSTRAINT; Schema: public; Owner: guest
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId");


--
-- Name: AspNetUserTokens PK_AspNetUserTokens; Type: CONSTRAINT; Schema: public; Owner: guest
--

ALTER TABLE ONLY public."AspNetUserTokens"
    ADD CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name");


--
-- Name: AspNetUsers PK_AspNetUsers; Type: CONSTRAINT; Schema: public; Owner: guest
--

ALTER TABLE ONLY public."AspNetUsers"
    ADD CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id");


--
-- Name: RefreshTokens PK_RefreshTokens; Type: CONSTRAINT; Schema: public; Owner: guest
--

ALTER TABLE ONLY public."RefreshTokens"
    ADD CONSTRAINT "PK_RefreshTokens" PRIMARY KEY ("Id");


--
-- Name: Users PK_Users; Type: CONSTRAINT; Schema: public; Owner: guest
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT "PK_Users" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: guest
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: EmailIndex; Type: INDEX; Schema: public; Owner: guest
--

CREATE INDEX "EmailIndex" ON public."AspNetUsers" USING btree ("NormalizedEmail");


--
-- Name: IX_AspNetRoleClaims_RoleId; Type: INDEX; Schema: public; Owner: guest
--

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON public."AspNetRoleClaims" USING btree ("RoleId");


--
-- Name: IX_AspNetUserClaims_UserId; Type: INDEX; Schema: public; Owner: guest
--

CREATE INDEX "IX_AspNetUserClaims_UserId" ON public."AspNetUserClaims" USING btree ("UserId");


--
-- Name: IX_AspNetUserLogins_UserId; Type: INDEX; Schema: public; Owner: guest
--

CREATE INDEX "IX_AspNetUserLogins_UserId" ON public."AspNetUserLogins" USING btree ("UserId");


--
-- Name: IX_AspNetUserRoles_RoleId; Type: INDEX; Schema: public; Owner: guest
--

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON public."AspNetUserRoles" USING btree ("RoleId");


--
-- Name: IX_RefreshTokens_UserId; Type: INDEX; Schema: public; Owner: guest
--

CREATE INDEX "IX_RefreshTokens_UserId" ON public."RefreshTokens" USING btree ("UserId");


--
-- Name: RoleNameIndex; Type: INDEX; Schema: public; Owner: guest
--

CREATE UNIQUE INDEX "RoleNameIndex" ON public."AspNetRoles" USING btree ("NormalizedName");


--
-- Name: UserNameIndex; Type: INDEX; Schema: public; Owner: guest
--

CREATE UNIQUE INDEX "UserNameIndex" ON public."AspNetUsers" USING btree ("NormalizedUserName");


--
-- Name: AspNetRoleClaims FK_AspNetRoleClaims_AspNetRoles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: guest
--

ALTER TABLE ONLY public."AspNetRoleClaims"
    ADD CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserClaims FK_AspNetUserClaims_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: guest
--

ALTER TABLE ONLY public."AspNetUserClaims"
    ADD CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserLogins FK_AspNetUserLogins_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: guest
--

ALTER TABLE ONLY public."AspNetUserLogins"
    ADD CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetRoles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: guest
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: guest
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserTokens FK_AspNetUserTokens_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: guest
--

ALTER TABLE ONLY public."AspNetUserTokens"
    ADD CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: RefreshTokens FK_RefreshTokens_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: guest
--

ALTER TABLE ONLY public."RefreshTokens"
    ADD CONSTRAINT "FK_RefreshTokens_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE USAGE ON SCHEMA public FROM PUBLIC;
GRANT ALL ON SCHEMA public TO PUBLIC;
GRANT ALL ON SCHEMA public TO guest;


--
-- PostgreSQL database dump complete
--

