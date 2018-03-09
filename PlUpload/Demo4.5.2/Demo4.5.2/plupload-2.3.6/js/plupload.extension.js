
(function ($, ko) {
	var jqElement = function (element) {
		var jq = $(element);
		if ($(document).find(element).length == 0) {  //处理元素在父页面执行的情况
			if ($(parent.document).find(element).length > 0)
				jq = parent.$(element);
		}
		return jq;
	};

	// 自定义mime类型
	var customMime = function(filters,mime){

		if (!filters.mime_types) {
			filters.mime_types = [];
		}

		var mimeTypes = $.isArray(mime) ? mime : (mime || '').split(',');
		if ($.inArray("image", mimeTypes)) {
			filters.mime_types.push({ title: "图片", extensions: "jpg,gif,png,jpeg,JPG,PNG,GIF,JPEG" });
		} else if ($.inArray("video", mimeTypes)) {
			filters.mime_types.push({ title: "视频", extensions: "mp4" });
		} else if ($.inArray("ppt", mimeTypes)) {
			filters.mime_types.push({ title: "PPT", extensions: "ppt,PPT,pptx,PPTX" });
		} else if ($.inArray("pdf", mimeTypes)) {
			filters.mime_types.push({ title: "PDF", extensions: "pdf,PDF" });
		} else if ($.inArray("rar", mimeTypes)) {
			filters.mime_types.push({ title: "压缩文件", extensions: "rar,zip,7z,RAR,ZIP,7Z" });
		}

		return filters;
	};

	var uploaderDefaultSetting = {
		//imageView:true,
		//singleUpload: true,
		maxFiles: 0, // 0 表示不限制
		autostart: false,
		folder: '/Upload/Common/',
		filesResult: ko.observableArray([]),
		mime: [],
		console_element: '#console',
		filelist_element: '#filelist',
		//drop_element: this.container || this.browse_button,

		filters: customMime({
			prevent_duplicates: true,
			max_file_size: '100mb',
			mime_types: []
		}, this.mime),

		runtimes: 'html5,flash,silverlight,html4',
        url: '/PLDemo/PLUpload',
        flash_swf_url: '/plupload-2.3.6/js/Moxie.swf',
        silverlight_xap_url: '/plupload-2.3.6/js/Moxie.xap',
		chunk_size: '100kb',
		file_data_name: 'postedFile',
		//multi_selection: false,

		// PreInit events, bound before the internal events
		preinit: {
			Init: function (up, info) {
				//console.log('[Init]', 'Info:', info, 'Features:', up.features);

				//debugger;
			},

			UploadFile: function (up, file) {
				//console.log('[UploadFile]', file);

				// You can override settings before the file is uploaded
				// up.setOption('url', 'upload.php?id=' + file.id);
				// up.setOption('multipart_params', {param1 : 'value1', param2 : 'value2'});

				//debugger;
				//multipart_params  property
				var paramsProperties = $.extend({},
					uploaderDefaultSetting.multipart_params, up.settings.multipart_params, {
					fileFlag: file.id,
					folder: up.settings.folder
				});
				up.setOption('multipart_params',paramsProperties);


			}
		},
		// Post init events, bound after the internal events
		init: {
			PostInit: function (up) {
				// Called after initialization is finished and internal event handlers bound
				//log('[PostInit]');

				//document.getElementById('uploadfiles').onclick = function () {
				//	uploader.start();
				//	return false;
				//};
				//debugger;

				var opt = up.getOption();
				var jq = jqElement(opt.filelist_element);

				plupload.each(opt.filesResult(), function (file) {
					debugger;
					var plFile = {
						id: file.newFileName,
						name: file.oldFileName,
						size: 0,
						destroy: function () {
						}
					}

					if (jq.length > 0) {
						var filename = plFile.name.length > 25 ? plFile.name.substr(0, 20) + "..." : plFile.name;
						var orignHtml = jq.html();
						var itemHtml = '<div id="' + plFile.id + '">' + filename + '  <b>100%</b></div>';
						jq.html(orignHtml + itemHtml);
					}

					up.files.push(plFile);

				});

			},
			FilesAdded: function (up, files) {
				debugger;
				//var filelist = up.settings.filelist_element;
				var opt = up.getOption();
				var jq = jqElement(opt.filelist_element);

				//单文件上传,逻辑为上传即替换原来的文件
				if (opt.singleUpload && opt.singleUpload === true && up.files.length > 1) {
					
					if (confirm("文件已经存在，是否替换？")) {
						// 删除原文件
						up.removeFile(up.files[0]);
						// 服务器上删除 =============================待完善============================================

						if (jq.length > 0) jq.html('');

					} else {
						plupload.each(files, function (file) {
							up.removeFile(file);
						});
						return false;
					}
				}

				// 上传数量控制
				if (opt.maxFiles && opt.maxFiles > 0 && (opt.maxFiles < files.length || opt.maxFiles < up.files.length)) {
					var msg = "\nError #-999: 超出允许上传的最大文件个数。";
					var jqConsole = jqElement(opt.console_element);
					if (jqConsole.length > 0) {
						jqConsole.html(msg);
					}
					else {
						alert(msg);
					}

					up.splice(opt.maxFiles, 999);
					return false;
				}

				if (jq.length > 0) {

					plupload.each(files, function (file) {
						var filename = file.name.length > 25 ? file.name.substr(0, 20) + "..." : file.name;
						var orignHtml = jq.html();
						var itemHtml = '<div id="' + file.id + '">' + filename + ' (' + plupload.formatSize(file.size) + ') <b></b></div>';
						jq.html(orignHtml + itemHtml);
					});
				}


				// 自动上传
				if (opt.autostart === true) {
					//up.start();
					setTimeout(up.start(), 1); // "detach" from the main thread
				}

			},
			UploadProgress: function (up, file) {

				var opt = up.getOption();
				var jq = jqElement(opt.console_element);
				if (jq.length > 0) {
					$('#' + file.id).find('b').html('<span>' + file.percent + "%</span>");
					var filename = file.name.length > 25 ? file.name.substr(0, 20) + "..." : file.name;
					jq.html("文件上传中：" + filename + "&nbsp;&nbsp;&nbsp;上传进度" + file.percent + "%");

				}
			},
            Error: function (up, err) {
                debugger;
				var opt = up.getOption();
				var jq = jqElement(opt.console_element);
				if (jq.length > 0) {
					jq.html("\nError #" + err.code + ": " + err.message);
				}
			},
			BeforeUpload: function (up, file) {
				// Called right before the upload for a given file starts, can be used to cancel it if required
				//log('[BeforeUpload]', 'File: ', file);
				//return false;

				//debugger;
				//console.log(up.settings.multipart_params);
			},
			FileUploaded: function (up, file, result) {

				debugger;
				var ret = eval("(" + result.response + ")");
				//alert(result);

				var opt = up.getOption();
				if(!!ret.result)
					opt.filesResult.push(ret);
			},
			UploadComplete: function (up, files) {
				// Called when all files are either uploaded or failed
				//log('[UploadComplete]');
            },
            ChunkUploaded: function (up, file, info) {
                //debugger
            }
        }

	};

	//uploader
	ko.bindingHandlers.uploader = {
		'init': function (element, valueAccessor, allBindings, viewModel, bindingContext) {//针对单个图片上传预览功能做拓展xaz
			//debugger;

		},
		'update': function (element, valueAccessor, allBindings, viewModel, bindingContext) {
			//debugger;
			var setting = ko.utils.unwrapObservable(valueAccessor()) || {};

			// 初始化控件
			var uploader = null;
			var jq = jqElement(element);
			var uploaderMain = $.data(jq, "uploader-uploader");//uploader-data
			if (uploaderMain) {
				uploader = uploaderMain;
			}
			else {

				//init property
				var intiProperties = uploaderDefaultSetting.init||{};
				if (setting.init) {
					intiProperties = $.extend({},intiProperties, setting.init);
				}

				// filters property
				var filtersProperties = uploaderDefaultSetting.filters || {};
				if (setting.filters) {
					filtersProperties = $.extend({}, filtersProperties, setting.filters);
				}


				var lastSetting = $.extend({}, uploaderDefaultSetting, { browse_button: element }, setting);
				lastSetting.init = intiProperties;
				lastSetting.filters = filtersProperties;

				// 单个文件上传控制
				if (setting.singleUpload && setting.singleUpload === true) {
					lastSetting.multi_selection = false;
					lastSetting.maxFiles = 1;
				}

				//// 文件上传结果接收
				//if (allBindings.has('value'))
				//	lastSetting.filesResult = allBindings.get('value') || ko.observableArray([]);


				uploader = new plupload.Uploader(lastSetting);
				uploader.init();

				$.data(jq, "uploader-uploader",uploader);
			}

		}
	};

})(jQuery, ko);