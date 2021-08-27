/**
 * @license Copyright (c) 2003-2021, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

// CKEDITOR.editorConfig = function( config ) {
// 	// Define changes to default configuration here.
// 	// For complete reference see:
// 	// https://ckeditor.com/docs/ckeditor4/latest/api/CKEDITOR_config.html

// 	// The toolbar groups arrangement, optimized for two toolbar rows.


CKEDITOR.editorConfig = function (config) {
	config.toolbarGroups = [{
			name: 'basicstyles',
			groups: ['basicstyles', 'cleanup']
		},
		{
			name: 'insert',
			groups: ['insert']
		},
		{
			name: 'links',
			groups: ['links']
		},
		{
			name: 'clipboard',
			groups: ['clipboard', 'undo']
		},
		{
			name: 'editing',
			groups: ['find', 'selection', 'spellchecker', 'editing']
		},
		{
			name: 'forms',
			groups: ['forms']
		},
		{
			name: 'tools',
			groups: ['tools']
		},
		{
			name: 'others',
			groups: ['others']
		},
		{
			name: 'paragraph',
			groups: ['list', 'indent', 'blocks', 'align', 'bidi', 'paragraph']
		},
		{
			name: 'styles',
			groups: ['styles']
		},
		{
			name: 'document',
			groups: ['mode', 'document', 'doctools']
		},
		{
			name: 'colors',
			groups: ['colors']
		},
		{
			name: 'about',
			groups: ['about']
		}
	];

	config.removeButtons = 'Underline,Subscript,Superscript,Scayt,Maximize,Strike,RemoveFormat,SpecialChar,Unlink,Anchor,About,Styles,Format';
	config.language = 'fa';
	config.uiColor = '#009DA0';
	config.height = 200;
	config.toolbarCanCollapse = true;
};