class backup
{
    void pub()
    {
        #region REPOSITORIO UTAL
        var u = "utal";
        if (src.Contains(u))
        {
            //10s
            //srt       rnk         date        popularity      author      title
            var rep = "http://utalca-primo.hosted.exlibrisgroup.com/primo_library/libweb/action/search.do?" +
            //$"fn=search&ct=search&indx={(pg - 1) * rpp + 1 - 10}&srt={srt_utal}&vid=UTALCA&vl(freeText0)={q}" +
            $"fn=search&ct=search&initialSearch=false&mode=Basic&tab=utalca_scope&pag=nxt&indx={(pg - 1) * rpp + 1 - 10}&dum=true&srt={srt_utal}&vid=UTALCA&frbg=&tb=t&vl(freeText0)={q}" +
            "&scp.scps=scope:(UTALCA),scope:(utalca_aleph),scope:(utalca_dspace),primo_central_multiple_fe&vl(1147761831UI1)=tesis&vl(1UIStartWith0)=contains&vl(1147761834UI0)=any";
            //"&vl(1147761831UI1)=tesis&vl(1UIStartWith0)=contains&vl(1147761834UI0)=any";

            var co = GetCo(u);
            try
            {
                var doc = await GetDoc(rep);
                string tmp = res.Match(doc.QuerySelector("em").TextContent).ToString().Replace(".", "");
                NoResults.TryAdd(u, GetNoResults(doc, "em", 0));
                var nodes = doc.QuerySelectorAll("tr.EXLResult");
                int len = (rpp > nodes.Count()) ? nodes.Count() : rpp;
                for (int i = 0; i < len; i++)
                {
                    try
                    {
                        var title = nodes[i].QuerySelector("h2.EXLResultTitle > a");
                        List<AuthorVM> autores = new List<AuthorVM>();
                        Enum.TryParse(nodes[i].QuerySelector("td.EXLThumbnail > span.EXLThumbnailCaption").TextContent, out Typep type);
                        string[] authors = nodes[i].QuerySelector("h3.EXLResultAuthor").TextContent.Split(';');
                        foreach (string author in authors)
                        {
                            string[] nn = author.Split(',');
                            if (nn.Length > 1)
                            {
                                autores.Add(new AuthorVM() { Last = nn[0], Name = nn[1] });
                            }
                        }
                        string journal = nodes[i].QuerySelector("span.EXLResultDetails").TextContent;
                        string[] formats = { "yyyy", "yyyy-MM", "yyyy-MM-dd" };
                        string year = string.IsNullOrEmpty(journal) ?
                            Regex.Match(nodes[i].QuerySelector("h3.EXLResultFourthLine").TextContent, "\\d{4}").Value :
                            Regex.Match(journal, "[^\\d]\\d{4},").Value.TrimEnd(',').TrimStart();
                        DateTime.TryParseExact(year,
                                                formats,
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.None,
                                                                                out DateTime Date);
                        PublicationVM pub = new PublicationVM()
                        {
                            Source = u,
                            Title = title.TextContent,
                            Uri = new Uri(new Uri(rep), title.Attributes["href"].Value),
                            Typep = type,
                            Authors = autores,
                            Date = Date,
                            Company = co,
                            Journal = journal,
                            //CompanyId = co.Id
                        };
                        Publications.Append(pub);
                    }
                    catch { continue; }
                }
            }
            catch
            {
            }
        }
        #endregion

        #region REPOSITORIO UST
        u = "ust";
        if (src.Contains(u))
        {
            //13s
            rep = $"http://www.ust.cl/investigacion/publicaciones/publicaciones-indexadas/page/{pg}/?nombre={q}";
            var co = GetCo(u);
            try
            {
                var doc = await GetDoc(rep);
                NoResults.TryAdd(u, GetNoResults(doc, "p.text-small > strong", 0));
                foreach (var node in doc.QuerySelectorAll("div.content-section.white-section.articulos-libros"))
                {
                    try
                    {
                        List<AuthorVM> autores = new List<AuthorVM>();
                        var authors = node.QuerySelectorAll("p");
                        string[] nn = authors[0].TextContent.Split(',');
                        autores.Add(new AuthorVM() { Last = Regex.Replace(nn[0], ".*>", ""), Name = nn[1].Trim() });
                        foreach (string author in Regex.Replace(authors[1].TextContent.TrimEnd('.'), ".*>", "").Split(","))
                        {
                            string[] nnn = author.Split(' ');
                            var autorito = new AuthorVM() { Last = nnn[0] };
                            try
                            {
                                autorito.Name = nnn[1];
                            }
                            catch { }
                            autores.Add(autorito);
                        }
                        Publications.Append(new PublicationVM()
                        {
                            Source = u,
                            Title = node.QuerySelector("h3").TextContent,
                            Uri = GetUri(node.QuerySelector("div > div > p > a")),
                            Authors = autores,
                            Date = GetDate(node, "div > div > p"),
                            Company = co
                        });
                    }
                    catch { continue; }
                }
            }
            catch
            {
            }
        }
        #endregion

        #region FAP
        u = "FAP";
        if (src.Contains(u))
        {
            rep = "http://www.fap.cl/controls/neochannels/neo_ch953/neochn953.aspx";

            try
            {
                HttpClient hc = new HttpClient();
                HttpResponseMessage result = await hc.GetAsync(rep);
                HtmlDocument doc = new HtmlDocument() { OptionReadEncoding = true };
                doc.Load(await result.Content.ReadAsStreamAsync());
                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//p/a");
                NoProjs.TryAdd(u, nodes.Count());
                int len = (rpp * pg.Value > nodes.Count()) ? nodes.Count() - 1 : rpp * pg.Value;
                int low = rpp * (pg.Value - 1);
                if (low <= len)
                {
                    for (int i = low; i < len; i++)
                    {
                        try
                        {
                            PublicationVM pub = new PublicationVM()
                            {
                                Source = u,
                                Typep = Typep.Proyecto,
                                Uri = new Uri(new Uri(rep), nodes[i].Attributes["href"].Value),
                                Title = nodes[i].InnerHtml,
                                Company = _context.Company.SingleOrDefault(c => c.Acronym == u.ToLower()),
                            };
                            Publications.Append(pub);
                        }
                        catch { continue; }
                    }
                }
            }
            catch
            {
            }
        }
        #endregion

        //https://ion.inapi.cl/Patente/ConsultaAvanzadaPatentes.aspx
        //http://www.inapi.cl/dominiopublico/

    }

	void agenda()
    {
        //Otros fondos http://investigacion.uc.cl/Fondos-concursables/concursos-externos.html

        //http://sitios.upla.cl/dapei/documentos/2013/2013_0422_guia_ff_cc.pdf

        //http://www.uss.cl/investigacion/agenda-de-concursos-de-investigacion-externos/

        //http://www.uchile.cl/portal/facultades-e-institutos/economia-y-negocios/academicos-y-publicaciones/depto-de-administracion/38505/fondos-concursables

        //INACH http://www.inach.cl/inach/?page_id=21918 div[@class='flexcol']//div[@class='flexcol']

        //FONDO CNTV 

        //FONDO Acceso a la Energía http://atencionciudadana.minenergia.cl/tramites/informacion/18/

        //COPEC UC
    }
}