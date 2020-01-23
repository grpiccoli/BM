using BiblioMit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiblioMit.Data
{
    public static class PostInitializer
    {
        public async static Task Initialize(ApplicationDbContext context, string admin)
        {
            #region Fora 7
            if (!context.Post.Any())
            {
            var Posts = new List<Post>()
            {
            new Post {Created=DateTime.Now, Title="Factores ambientales que impactan la captación de semillas", Content="Programas de monitoreo ambiental y dinamicas de bancos naturales, reproductivo, larval con impacto en la captación de semillas",ForumId=1,UserId=admin},
            new Post {Created=DateTime.Now, Title="Mejorar el conocimiento de los ciclos reproductivos del chorito", Content="Proyectos I+D+i y del Tipo PDT o PTT. Ciclos reproductivos y transferencias de resultados",ForumId=1,UserId=admin},
            new Post {Created=DateTime.Now, Title="Manejo productivo con bajo impacto para la semilla", Content="Proyectos PDT o PTT. Estrategias de manejo  eficiente y de bajo impacto para la semilla",ForumId=1,UserId=admin},

            new Post {Created=DateTime.Now, Title="Protocolo público privado de mitigación económica y productiva durante FAN", Content="Programa de Reactivación urbana y productiva para la coordinación publico- privadas involucradas en la toma de deciciones FANs",ForumId=2,UserId=admin},
            new Post {Created=DateTime.Now, Title="Optimización (tecnología y manejos) en la siembra y engorda de materia prima", Content="Proyectos I+D, PDT o PTT. Investigación que permita optimar la eficiencia en la siembra y engorda de choritos",ForumId=2,UserId=admin},
            new Post {Created=DateTime.Now, Title="Aumento en la capacidad de predicción FAN", Content="Programas de monitoreo ambiental global y local que impactan sobre la frecuencia y distribución de FANs que permitan generar un modelo predictivo robusto",ForumId=2,UserId=admin},

            new Post {Created=DateTime.Now, Title="Generación de conocimiento respecto a certificaciones ambientales y de sustentabilidad", Content="Proyecto PDP, APL, que permitan generar conocimientos y apoyo MIPES para obtener certificaciones ambientales y sustentabilidad",ForumId=3,UserId=admin},
            new Post {Created=DateTime.Now, Title="Optimización de la gestión y valorización de los desechos de la industria", Content="Proyectos I+D+i y del Tipo PDT o PTT. Asociados al reciclaje eco-eficiente de residuos generados por la industria mitilicultora",ForumId=3,UserId=admin},
            new Post {Created=DateTime.Now, Title="Efectos de Cambio Climático sobre la actividad mitilicultora y sus productos", Content="Programas Programas de monitoreo ambiental permanetes y estudos en laboratorio que permitan predecir el impacto real del cambio climático sobre la adecuación en las conductas productivas de largo plazo",ForumId=3,UserId=admin},

            new Post {Created=DateTime.Now, Title="Mejoras a las Estructuras de Cultivo", Content="Proyectos PDT y/o PTT, CETMIS. Desarrollo y transferencia de Estructuras de captación y cultivos de choritos",ForumId=4,UserId=admin},
            new Post {Created=DateTime.Now, Title="Exploración de Alternativas de Automatización", Content="Proyectos PDT y/o PTT, CETMIS. Desarrollo y transferencia para la automatización para la captación y cultivos de choritos",ForumId=4,UserId=admin},
            new Post {Created=DateTime.Now, Title="Generación de conocimiento sobre sistemas productivas más eficientes", Content="Proyectos PDT y/o PTT, CETMIS. Desarrollo y transferencia generación de intrumentos productivos más eficiente",ForumId=4,UserId=admin},

            new Post {Created=DateTime.Now, Title="Investigación que permita incrementar el consumo nacional de Choritos", Content="Proyectos regionales para la competitividad, que resalte los atributos de choritos de cultivo y consumo nacional.",ForumId=5,UserId=admin},
            new Post {Created=DateTime.Now, Title="Investigaciones orientadas a la búsqueda de nuevos mercados", Content="Proyectos tipo ProChile, que descubra nuevos mercados internacionales",ForumId=5,UserId=admin},
            new Post {Created=DateTime.Now, Title="Generar identidad de la comunidad con la industria", Content="Programa de Difución Institucional, que permita generar puente de identidad de la industria mitilicultora con la comunidad regional",ForumId=5,UserId=admin},

            new Post {Created=DateTime.Now, Title="Estudios de los  atributos nutrionales del mejillón para la salud humana", Content="Proyectos I+D+i y del Tipo PDT o PTT. Elevar la categoría del mejollón chileno como un Súper Alimento.",ForumId=6,UserId=admin},
            new Post {Created=DateTime.Now, Title="Mejoras y estandarización de indicadores de calidad de MP.", Content="NCh de Estandarización. Mejorar las relaciones comerciales a través de criterios estandar de semillas y chorito de cosecha",ForumId=6,UserId=admin},
            new Post {Created=DateTime.Now, Title="Conocer los hábitos de consumo en grupos de consumidores concretos", Content="Proyectos regionales para la competitividad. Perfil de consumidores para generar productos diferenciados por grupos etarios y socio económicos",ForumId=6,UserId=admin},
            };
            await context.BulkInsertAsync(Posts).ConfigureAwait(false);
            }
            return;
            #endregion 16 16
        }
    }
}
