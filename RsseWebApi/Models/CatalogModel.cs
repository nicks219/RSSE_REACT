using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Data;
using RandomSongSearchEngine.DTO;

namespace RandomSongSearchEngine.Models
{
    /// <summary>
    /// ������� �����
    /// </summary>
    public class CatalogModel : ICatalogModel
    {
        public IServiceScopeFactory ServiceScopeFactory { get; set; }
        public ILogger<SongModel> Logger { get; set; }

        //����������� ��� ���������� ����� ��� ������ ��������������

        public CatalogModel(IServiceScopeFactory serviceScopeFactory, ILogger<SongModel> logger)
        {
            ServiceScopeFactory = serviceScopeFactory;
            Logger = logger;
            //������ ������ ����� songscount - ����� �������� ����� �� ������ ��� ������� ������ ���
            //���������� ����� ��������!
            GetSongCount();
        }

        //� ParentModel ���� ����
        /// <summary>
        /// ����������� ID ������ ��� �������� ����� �������
        /// </summary>
        public int SavedTextId { get; set; }

        /// <summary>
        /// ������ �� �������� ����� � ��������������� �� ID ��� CatalogView
        /// </summary>
        public List<Tuple<string, int>> TitlesAndIds { get; set; }

        /// <summary>
        /// ������������ ��� ����������� ����� ������ ���� ������ �� �����
        /// </summary>
        public List<int> NavigationButtons { get; set; }

        /// <summary>
        /// ���������� ����� � ���� ������
        /// </summary>
        public int SongsCount { get; set; }

        /// <summary>
        /// ����� ��������� ������������� ��������
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// ���������� ����� �� ����� ��������
        /// </summary>
        public readonly int PageSize = 10;

        private void GetSongCount ()
        {
            try
            {
                using var scope = ServiceScopeFactory.CreateScope();//
                var database = scope.ServiceProvider.GetRequiredService<RsseContext>();//
                SongsCount = database.Text.Count();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "[CatalogModel]: no database");
                //� ������ ���������� �� �� �� ����� � null referenece exception ��-�� TitleAndTextID
                TitlesAndIds = new List<Tuple<string, int>>();
            }
        }
    }
}
