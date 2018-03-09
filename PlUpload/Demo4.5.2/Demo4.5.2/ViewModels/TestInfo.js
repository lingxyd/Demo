
function TestInfo(win, vData) {
    var self = this;
    var curData = vData || {};
    var query = $ || parent.$;
    //debugger;
    this.form = ko.mapping.fromJS(curData);
    // 重新设置
    this.form.BusinessPlanPath = ko.observable();
    this.form.BusinessPlanNewFileName = ko.observable();
    this.form.BusinessPlanOldFileName = ko.observable();

    ////debugger;
    //self.form.IPRSEidt([
    //    { Id: 1, Name: 'test1', Number: 'Number1', Person: 'Person1' },
    //    { Id: 2, Name: 'test2', Number: 'Number2', Person: 'Person2' },
    //    { Id: 3, Name: 'test3', Number: 'Number3', Person: 'Person3' }]);


    //// 知识产权
    //this.IPRSArray = ko.observableArray();
    //if (!!self.form.IPRSEidt() && self.form.IPRSEidt().length > 0) {
    //    self.IPRSArray(self.form.IPRSEidt());
    //}
    //else {
    //    self.IPRSArray.push({ Id: 0, Name: '', Number: '', Person: '' });
    //}
    //this.addIPRSRowClick = function (data, event) {
    //    self.IPRSArray.push({ Id: 0, Name: '', Number: '', Person: '' });

    //    ////获取高度
    //    //var height = $('.applyCon').height();
    //    //$('.footer').css('margin-top', height);
    //};
    //this.deleteIPRSRowClick = function (data, event) {
    //    self.IPRSArray.remove(data);
    //};
    //// 知识产权后 重新render
    //this.elementParser = function (element, data) {
    //    $.parser.parse($(element).filter("div"));
    //};

    this.CustomValidate = function () {
        debugger
        // 手动验证数据
        // 商业计划书
        if (!self.form.BusinessPlanNewFileName()) {
            com.message('warning', '请上传商业计划书！');
            return false;
        }

        return true;
    }

    this.saveClick = function (data, event) {

        // iprs
        self.form.IPRSEidt(self.IPRSArray());

        if (self.CustomValidate() && com.formValidate()) {
            com.ajax({
                url: '/SignUp/TeamInfoSave',
                data: ko.toJSON(ko.mapping.toJS(self.form)),
                success: function (r) {
                    if (com.ajaxResultValidate(r)) {
                        if (r.result) {
                            window.location.href = '/SignUp/SignUpResult/';
                            //com.message('success', '保存成功！点击确定，进行下一步...', function () { window.location.href = '/SignUp/TeamInfo/'; });
                        }
                        else {
                            com.message('error', '保存失败！\n' + r.result_text);
                        }
                    }
                }
            });
        }

    };


    // 商业计划书
    this.orignBusinessPlanResult = ko.observableArray([]);
    //if (!!self.form.BusinessPlanNewFileName())
    //    self.orignBusinessPlanResult.push({
    //        newFilePath: self.form.BusinessPlanPath(),
    //        newFileName: self.form.BusinessPlanNewFileName(),
    //        oldFileName: self.form.BusinessPlanOldFileName(),
    //        result: true,
    //        result_text: ''
    //    });

    this.BusinessPlan = {
        filesResult: self.orignBusinessPlanResult,
        singleUpload: true,
        //multi_selection: false,
        autostart: true,
        chunk_size: 0,
        //chunk_size: '10mb',
        filters: {
            prevent_duplicates: false,
            max_file_size: '100mb',
            mime_types: [
                { title: "PDF", extensions: "pdf,PDF" },
                { title: "PPT", extensions: "ppt,PPT,pptx,PPTX" },
                { title: "word", extensions: "doc,DOC,docx,DOCX" },
            ]
        },
        init: {
            UploadComplete: function (up, files) {
                // Called when all files are either uploaded or failed
                //log('[UploadComplete]');
                debugger;
                var opt = up.getOption();
                var filesResult = opt.filesResult();
                //console.log(filesresult);

                self.form.BusinessPlanPath(filesResult[0].newFilePath);
                self.form.BusinessPlanNewFileName(filesResult[0].newFileName);
                self.form.BusinessPlanOldFileName(filesResult[0].oldFileName);

            }
        }
    };

}